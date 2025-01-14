// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Aggregates;
using Cratis.Chronicle.Events;

namespace Cratis.Chronicle.Orleans.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootFactory"/>.
/// </summary>
/// <param name="grainFactory"><see cref="IGrainFactory"/> for creating grains.</param>
public class AggregateRootFactory(IGrainFactory grainFactory) : IAggregateRootFactory
{
    /// <inheritdoc/>
    public Task<TAggregateRoot> Get<TAggregateRoot>(EventSourceId id, EventStreamId? streamId = default, EventSourceType? eventSourceType = default)
        where TAggregateRoot : class, Chronicle.Aggregates.IAggregateRoot
    {
        WrongTypeForAggregateRoot.ThrowIfWrongType(typeof(TAggregateRoot));
        var key = new AggregateRootKey(eventSourceType ?? EventSourceType.Default, id, streamId ?? EventStreamId.Default);
        var aggregateRoot = (grainFactory.GetGrain(typeof(TAggregateRoot), (string)key) as TAggregateRoot)!;
        return Task.FromResult(aggregateRoot);
    }
}
