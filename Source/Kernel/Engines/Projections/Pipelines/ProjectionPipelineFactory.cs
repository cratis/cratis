// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Changes;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Kernel.Engines.Changes;
using Aksio.Cratis.Kernel.Engines.Sinks;
using Aksio.Cratis.Projections.Definitions;
using Aksio.Cratis.Schemas;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Engines.Projections.Pipelines;

/// <summary>
/// Represents an implementation of <see cref="IProjectionPipelineFactory"/>.
/// </summary>
public class ProjectionPipelineFactory : IProjectionPipelineFactory
{
    readonly ISinks _projectionSinks;
    readonly IEventSequenceStorage _eventProvider;
    readonly IObjectComparer _objectComparer;
    readonly IChangesetStorage _changesetStorage;
    readonly ITypeFormats _typeFormats;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionPipelineFactory"/> class.
    /// </summary>
    /// <param name="projectionSinks"><see cref="ISinks"/> in the system.</param>
    /// <param name="eventProvider"><see cref="IEventSequenceStorage"/> in the system.</param>
    /// <param name="objectComparer"><see cref="IObjectComparer"/> for comparing objects.</param>
    /// <param name="changesetStorage"><see cref="IChangesetStorage"/> for storing changesets as they occur.</param>
    /// <param name="typeFormats"><see cref="ITypeFormats"/> for resolving actual CLR types for schemas.</param>
    /// <param name="loggerFactory"><see cref="ILoggerFactory"/> for creating loggers.</param>
    public ProjectionPipelineFactory(
        ISinks projectionSinks,
        IEventSequenceStorage eventProvider,
        IObjectComparer objectComparer,
        IChangesetStorage changesetStorage,
        ITypeFormats typeFormats,
        ILoggerFactory loggerFactory)
    {
        _projectionSinks = projectionSinks;
        _eventProvider = eventProvider;
        _objectComparer = objectComparer;
        _changesetStorage = changesetStorage;
        _typeFormats = typeFormats;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public IProjectionPipeline CreateFrom(IProjection projection, ProjectionPipelineDefinition definition)
    {
        ISink sink = default!;
        if (definition.Sinks.Any())
        {
            var sinkDefinition = definition.Sinks.First();
            sink = _projectionSinks.GetForTypeAndModel(sinkDefinition.TypeId, projection.Model);
        }

        return new ProjectionPipeline(
            projection,
            _eventProvider,
            sink,
            _objectComparer,
            _changesetStorage,
            _typeFormats,
            _loggerFactory.CreateLogger<ProjectionPipeline>());
    }
}
