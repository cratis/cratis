// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Connections;
using Aksio.Cratis.Observation;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Grains.Observation.Clients;

/// <summary>
/// Represents an implementation of <see cref="IClientObservers"/>.
/// </summary>
public class ClientObservers : Grain, IClientObservers
{
    readonly ILogger<ClientObservers> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientObservers"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ClientObservers(ILogger<ClientObservers> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Register(ConnectionId connectionId, IEnumerable<ClientObserverRegistration> registrations, IEnumerable<TenantId> tenants)
    {
        _logger.RegisterObservers();

        var microserviceId = (MicroserviceId)this.GetPrimaryKey();

        var registrationTasks = new List<Task>();

        foreach (var registration in registrations)
        {
            foreach (var tenantId in tenants)
            {
                _logger.RegisterObserver(
                    registration.ObserverId,
                    registration.Name,
                    registration.EventSequenceId);
                var key = new ObserverKey(microserviceId, tenantId, registration.EventSequenceId);
                var observer = GrainFactory.GetGrain<IClientObserver>(registration.ObserverId, key);
                registrationTasks.Add(observer.Start(registration.Name, connectionId, registration.EventTypes));
            }
        }

        await Task.WhenAll(registrationTasks);
    }
}