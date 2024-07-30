// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Events;
using Cratis.Chronicle.EventSequences;
using Cratis.Chronicle.Projections;
using Cratis.Chronicle.Reactions;
using Cratis.Chronicle.Reducers;

namespace Cratis.Chronicle.XUnit.Events;

/// <summary>
/// Represents an implementation of <see cref="IEventStore"/> for testing.
/// </summary>
public class EventStoreForTesting : IEventStore
{
    readonly EventSequenceForTesting _nullEventSequence = new();

    /// <inheritdoc/>
    public EventStoreName Name => nameof(EventStoreForTesting);

    /// <inheritdoc/>
    public EventStoreNamespaceName Namespace => "Default";

    /// <inheritdoc/>
    public IChronicleConnection Connection => throw new NotImplementedException();

    /// <inheritdoc/>
    public Chronicle.Aggregates.IAggregateRootFactory AggregateRootFactory => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEventTypes EventTypes => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEventLog EventLog => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReactions Reactions => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReducers Reducers => throw new NotImplementedException();

    /// <inheritdoc/>
    public IProjections Projections => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task DiscoverAll() => Task.CompletedTask;

    /// <inheritdoc/>
    public IEventSequence GetEventSequence(EventSequenceId id) => _nullEventSequence;

    /// <inheritdoc/>
    public Task RegisterAll() => Task.CompletedTask;
}
