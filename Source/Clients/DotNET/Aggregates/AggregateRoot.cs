// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Aksio.Cratis.Auditing;
using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;

namespace Aksio.Cratis.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRoot"/>.
/// </summary>
public class AggregateRoot : IAggregateRoot
{
    /// <summary>
    /// The causation aggregate root type property.
    /// </summary>
    public const string CausationAggregateRootTypeProperty = "aggregateRootType";

    /// <summary>
    /// The event sequence id causation property.
    /// </summary>
    public const string CausationEventSequenceIdProperty = "eventSequenceId";

    /// <summary>
    /// The causation type for the aggregate root.
    /// </summary>
    public static readonly CausationType CausationType = "AggregateRoot";

    /// <summary>
    /// Cratis Internal: The event handlers for the aggregate root.
    /// </summary>
    internal IAggregateRootEventHandlers EventHandlers = default!;

    /// <summary>
    /// Cratis Internal: The event sequence for the aggregate root.
    /// </summary>
    internal IEventSequence EventSequence = default!;

    /// <summary>
    /// Cratis Internal: The <see cref="ICausationManager"/> to use with the aggregate root when committing.
    /// </summary>
    internal ICausationManager CausationManager = default!;

    /// <summary>
    /// Cratis Internal: The <see cref="_eventSourceId"/> for the aggregate root.
    /// </summary>
    internal EventSourceId _eventSourceId = EventSourceId.Unspecified;

    readonly List<object> _uncommittedEvents = new();

    /// <inheritdoc/>
    public virtual bool IsStateful => false;

    /// <inheritdoc/>
    public virtual EventSequenceId EventSequenceId => EventSequenceId.Log;

    /// <summary>
    /// Cratis Internal: The type of state for the aggregate root.
    /// </summary>
    internal virtual Type StateType => typeof(object);

    /// <summary>
    /// Gets the <see cref="EventSourceId"/> for the aggregate root.
    /// </summary>
    protected EventSourceId EventSourceId => _eventSourceId;

    /// <inheritdoc/>
    public async Task Apply<T>(T @event)
        where T : class
    {
        typeof(T).ValidateEventType();
        _uncommittedEvents.Add(@event);
        await EventHandlers.Handle(this, new[] { new EventAndContext(@event, EventContext.From(EventSourceId, EventSequenceNumber.Unavailable)) });
    }

    /// <inheritdoc/>
    public async Task<AggregateRootCommitResult> Commit()
    {
        CausationManager.Add(CausationType, new Dictionary<string, string>
        {
            { CausationAggregateRootTypeProperty, GetType().AssemblyQualifiedName! },
            { CausationEventSequenceIdProperty, EventSequenceId.ToString() }
        });

        await EventSequence.AppendMany(_eventSourceId, _uncommittedEvents.ToArray());

        var result = new AggregateRootCommitResult(true, _uncommittedEvents.ToImmutableList());
        _uncommittedEvents.Clear();
        return result;
    }

    /// <summary>
    /// Cratis Internal: Set the state for the aggregate root.
    /// </summary>
    /// <param name="state">State to set.</param>
    internal virtual void SetState(object state)
    {
    }
}

/// <summary>
/// Represents a stateful implementation of <see cref="IAggregateRoot"/>.
/// </summary>
/// <typeparam name="TState">Type of state for the aggregate root.</typeparam>
public class AggregateRoot<TState> : AggregateRoot
    where TState : class
{
    /// <summary>
    /// State of the aggregate root - accessible only to Cratis internally.
    /// </summary>
    internal TState _state = default!;

    /// <inheritdoc/>
    public override bool IsStateful => true;

    /// <inheritdoc/>
    internal override Type StateType => typeof(TState);

    /// <summary>
    /// Gets the current state of the aggregate root.
    /// </summary>
    protected TState State => _state;

    /// <inheritdoc/>
    internal override void SetState(object state) => _state = (state as TState)!;
}