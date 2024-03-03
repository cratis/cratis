// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Events;
using Cratis.EventSequences;
using Cratis.EventSequences.Inboxes;
using Cratis.Kernel.Grains.Observation;
using Cratis.Observation;
using Orleans.Runtime;

namespace Cratis.Kernel.Grains.EventSequences.Inbox;

/// <summary>
/// Represents an implementation of <see cref="IInbox"/>.
/// </summary>
public class Inbox : Grain, IInbox
{
    readonly ILocalSiloDetails _localSiloDetails;

    /// <summary>
    /// Initializes a new instance of the <see cref="Inbox"/> class.
    /// </summary>
    /// <param name="localSiloDetails"><see cref="ILocalSiloDetails"/> for getting information about the silo this grain is on.</param>
    public Inbox(ILocalSiloDetails localSiloDetails)
    {
        _localSiloDetails = localSiloDetails;
    }

    /// <inheritdoc/>
    public async Task Start()
    {
        var microserviceId = this.GetPrimaryKey(out var keyAsString);
        var key = InboxKey.Parse(keyAsString);

        var observer = GrainFactory.GetGrain<IObserver>(
            microserviceId,
            new ObserverKey(
                microserviceId,
                key.TenantId,
                EventSequenceId.Outbox,
                key.MicroserviceId,
                key.TenantId));

        var name = $"Inbox for {microserviceId}, Outbox from {key.MicroserviceId} for Tenant {key.TenantId}";
        await observer.Subscribe<IInboxObserverSubscriber>(name, ObserverType.Inbox, Enumerable.Empty<EventType>(), _localSiloDetails.SiloAddress);
    }
}
