// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Connections;
using Aksio.Cratis.Kernel.Grains.Clients;
using Aksio.Cratis.Observation;
using Aksio.Cratis.Observation.Reducers;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Aksio.Cratis.Kernel.Grains.Observation.Reducers.Clients;

/// <summary>
/// Represents an implementation of <see cref="IClientReducer"/>.
/// </summary>
public class ClientReducer : Grain, IClientReducer, INotifyClientDisconnected
{
    readonly ILogger<ClientReducer> _logger;
    readonly IExecutionContextManager _executionContextManager;
    readonly ILocalSiloDetails _localSiloDetails;
    ObserverId? _reducerId;
    ObserverKey? _observerKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientReducer"/>.
    /// </summary>
    /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/>.</param>
    /// <param name="localSiloDetails"><see cref="ILocalSiloDetails"/> for getting information about the silo this grain is on.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ClientReducer(
        IExecutionContextManager executionContextManager,
        ILocalSiloDetails localSiloDetails,
        ILogger<ClientReducer> logger)
    {
        _executionContextManager = executionContextManager;
        _localSiloDetails = localSiloDetails;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _reducerId = this.GetPrimaryKey(out var keyAsString);
        _observerKey = ObserverKey.Parse(keyAsString);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task Start(ObserverName name, ConnectionId connectionId, IEnumerable<EventTypeWithKeyExpression> eventTypes)
    {
        _executionContextManager.Establish(_observerKey!.TenantId, CorrelationId.New(), _observerKey!.MicroserviceId);
        _logger.Starting(_observerKey!.MicroserviceId, _reducerId!, _observerKey!.EventSequenceId, _observerKey!.TenantId);
        var observer = GrainFactory.GetGrain<IObserver>(_reducerId!, _observerKey!);
        var connectedClients = GrainFactory.GetGrain<IConnectedClients>(0);

        RegisterTimer(HandleConnectedClientsSubscription, null!, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        var connectedClient = await connectedClients.GetConnectedClient(connectionId);
        await observer.Subscribe<IClientReducerSubscriber>(
            name,
            ObserverType.Reducer,
            eventTypes.Select(_ => _.EventType).ToArray(),
            _localSiloDetails.SiloAddress,
            connectedClient);
    }

    /// <inheritdoc/>
    public void OnClientDisconnected(ConnectedClient client)
    {
        _logger.ClientDisconnected(client.ConnectionId, _observerKey!.MicroserviceId, _reducerId!, _observerKey!.EventSequenceId, _observerKey!.TenantId);
        var id = this.GetPrimaryKey(out var keyAsString);
        var key = ObserverKey.Parse(keyAsString);
        var observer = GrainFactory.GetGrain<IObserver>(id, key);
        observer.Unsubscribe();
    }

    async Task HandleConnectedClientsSubscription(object state)
    {
        var connectedClients = GrainFactory.GetGrain<IConnectedClients>(0);
        await connectedClients.SubscribeDisconnected(this.AsReference<INotifyClientDisconnected>());
    }
}
