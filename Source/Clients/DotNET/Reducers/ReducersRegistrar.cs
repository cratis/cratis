// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Aksio.Cratis.Models;
using Aksio.Cratis.Observation.Reducers;
using Aksio.Cratis.Projections.Definitions;
using Aksio.Cratis.Schemas;
using Aksio.Cratis.Sinks;
using Aksio.Reflection;
using Cratis.Chronicle.Aggregates;
using Cratis.Chronicle.Connections;
using Cratis.Chronicle.Events;
using Cratis.Chronicle.Observation;
using Microsoft.Extensions.Logging;

namespace Cratis.Chronicle.Reducers;

/// <summary>
/// Represents an implementation of <see cref="IReducersRegistrar"/>.
/// </summary>
public class ReducersRegistrar : IReducersRegistrar
{
    readonly IEnumerable<Type> _aggregateRootStateTypes;
    readonly IDictionary<Type, IReducerHandler> _handlers;
    readonly IExecutionContextManager _executionContextManager;
    readonly IModelNameResolver _modelNameResolver;
    readonly IJsonSchemaGenerator _jsonSchemaGenerator;
    readonly IConnection _connection;
    readonly ILogger<ReducersRegistrar> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ReducersRegistrar"/>.
    /// </summary>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for establishing execution context.</param>
    /// <param name="clientArtifacts">Optional <see cref="IClientArtifactsProvider"/> for the client artifacts.</param>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> to get instances of types.</param>
    /// <param name="reducerValidator"><see cref="IReducerValidator"/> for validating reducer types.</param>
    /// <param name="eventTypes">Registered <see cref="IEventTypes"/>.</param>
    /// <param name="eventSerializer"><see cref="IEventSerializer"/> for serializing of events.</param>
    /// <param name="modelNameResolver"><see cref="IModelNameResolver"/> for resolving read model names.</param>
    /// <param name="jsonSchemaGenerator"><see cref="IJsonSchemaGenerator"/> for generating JSON schemas.</param>
    /// <param name="connection"><see cref="IConnection"/> for working with kernel.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ReducersRegistrar(
        IExecutionContextManager executionContextManager,
        IClientArtifactsProvider clientArtifacts,
        IServiceProvider serviceProvider,
        IReducerValidator reducerValidator,
        IEventTypes eventTypes,
        IEventSerializer eventSerializer,
        IModelNameResolver modelNameResolver,
        IJsonSchemaGenerator jsonSchemaGenerator,
        IConnection connection,
        ILogger<ReducersRegistrar> logger)
    {
        _aggregateRootStateTypes = clientArtifacts
                                            .AggregateRoots
                                            .SelectMany(_ => _.AllBaseAndImplementingTypes())
                                            .Where(_ => _.IsDerivedFromOpenGeneric(typeof(AggregateRoot<>)))
                                            .Select(_ => _.GetGenericArguments()[0])
                                            .ToArray();

        _handlers = clientArtifacts.Reducers
                            .ToDictionary(
                                _ => _,
                                reducerType =>
                                {
                                    var readModelType = reducerType.GetReadModelType();
                                    reducerValidator.Validate(reducerType);
                                    var reducer = reducerType.GetCustomAttribute<ReducerAttribute>()!;
                                    return new ReducerHandler(
                                        reducer.ReducerId,
                                        reducerType.FullName ?? $"{reducerType.Namespace}.{reducerType.Name}",
                                        reducer.EventSequenceId,
                                        new ReducerInvoker(
                                            serviceProvider,
                                            eventTypes,
                                            reducerType,
                                            readModelType),
                                        eventSerializer,
                                        ShouldReducerBeActive(readModelType, reducer)) as IReducerHandler;
                                });
        _executionContextManager = executionContextManager;
        _modelNameResolver = modelNameResolver;
        _jsonSchemaGenerator = jsonSchemaGenerator;
        _connection = connection;
        _logger = logger;
    }

    /// <inheritdoc/>
    public IEnumerable<IReducerHandler> GetAll() => _handlers.Values;

    /// <inheritdoc/>
    public IReducerHandler GetById(ReducerId reducerId)
    {
        var reducer = _handlers.Values.SingleOrDefault(_ => _.ReducerId == reducerId);
        ReducerDoesNotExist.ThrowIfDoesNotExist(reducerId, reducer);
        return reducer!;
    }

    /// <inheritdoc/>
    public IReducerHandler GetByType(Type reducerType)
    {
        ThrowIfTypeIsNotAnObserver(reducerType);
        return _handlers[reducerType];
    }

    /// <inheritdoc/>
    public Type GetClrType(ReducerId reducerId)
    {
        var reducer = _handlers.SingleOrDefault(_ => _.Value.ReducerId == reducerId);
        ReducerDoesNotExist.ThrowIfDoesNotExist(reducerId, reducer.Value);
        return reducer.Key;
    }

    /// <inheritdoc/>
    public IReducerHandler GetForModelType(Type modelType) => _handlers[modelType];

    /// <inheritdoc/>
    public bool HasReducerFor(Type modelType) => _handlers.ContainsKey(modelType);

    /// <inheritdoc/>
    public async Task Initialize()
    {
        _logger.RegisterReducers();

        foreach (var reducerHandler in _handlers.Values)
        {
            _logger.RegisterReducer(
                reducerHandler.ReducerId,
                reducerHandler.Name,
                reducerHandler.EventSequenceId);
        }

        var microserviceId = _executionContextManager.Current.MicroserviceId;
        var route = $"/api/events/store/{microserviceId}/reducers/register/{_connection.ConnectionId}";

        var registrations = _handlers.Values
            .Where(_ => _.IsActive)
            .Select(_ => new ReducerDefinition(
                _.ReducerId,
                _.Name,
                _.EventSequenceId,
                _.EventTypes.Select(et => new EventTypeWithKeyExpression(et, "$eventSourceId")).ToArray(),
                new ModelDefinition(
                    _modelNameResolver.GetNameFor(_.ReadModelType),
                    _jsonSchemaGenerator.Generate(_.ReadModelType).ToJson()),
                WellKnownSinkTypes.MongoDB)).ToArray();

        await _connection.PerformCommand(route, registrations);
    }

    bool ShouldReducerBeActive(Type readModelType, ReducerAttribute reducerAttribute)
    {
        if (!reducerAttribute.IsActive || _aggregateRootStateTypes.Contains(readModelType))
        {
            return false;
        }

        return reducerAttribute.IsActive;
    }

    void ThrowIfTypeIsNotAnObserver(Type reducerType)
    {
        if (!_handlers.ContainsKey(reducerType))
        {
            throw new UnknownReducerType(reducerType);
        }
    }
}
