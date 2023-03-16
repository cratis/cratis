// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Clients;
using Aksio.Cratis.Events;
using Aksio.Cratis.Execution;

namespace Aksio.Cratis.EventSequences;

/// <summary>
/// Represents an implementation of <see cref="IEventSequence"/>.
/// </summary>
public class EventSequence : IEventSequence
{
    readonly EventSequenceId _eventSequenceId;
    readonly IEventTypes _eventTypes;
    readonly IEventSerializer _eventSerializer;
    readonly IClient _client;
    readonly IExecutionContextManager _executionContextManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequence"/> class.
    /// </summary>
    /// <param name="eventSequenceId">The event sequence it represents.</param>
    /// <param name="eventTypes">Known <see cref="IEventTypes"/>.</param>
    /// <param name="eventSerializer">The <see cref="IEventSerializer"/> for serializing events.</param>
    /// <param name="client"><see cref="IClient"/> for getting connections.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with the execution context.</param>
    public EventSequence(
        EventSequenceId eventSequenceId,
        IEventTypes eventTypes,
        IEventSerializer eventSerializer,
        IClient client,
        IExecutionContextManager executionContextManager)
    {
        _eventSequenceId = eventSequenceId;
        _eventTypes = eventTypes;
        _eventSerializer = eventSerializer;
        _client = client;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    public async Task<EventSequenceNumber> GetNextSequenceNumber()
    {
        var route = $"{GetBaseRoute()}/next-sequence-number";
        var result = await _client.PerformQuery<EventSequenceNumber>(route);
        return result.Data;
    }

    /// <inheritdoc/>
    public async Task<EventSequenceNumber> GetTailSequenceNumber()
    {
        var route = $"{GetBaseRoute()}/tail-sequence-number";
        var result = await _client.PerformQuery<EventSequenceNumber>(route);
        return result.Data;
    }

    /// <inheritdoc/>
    public async Task Append(EventSourceId eventSourceId, object @event, DateTimeOffset? validFrom = null)
    {
        var eventType = _eventTypes.GetEventTypeFor(@event.GetType());
        var serializedEvent = await _eventSerializer.Serialize(@event);
        var payload = new AppendEvent(eventSourceId, eventType, serializedEvent, validFrom);
        var route = $"{GetBaseRoute()}";
        await _client.PerformCommand(route, payload);
    }

    /// <inheritdoc/>
    public async Task Redact(EventSequenceNumber sequenceNumber, RedactionReason? reason = default)
    {
        reason ??= RedactionReason.Unknown;
        var payload = new RedactEvent(sequenceNumber, reason);
        var route = $"{GetBaseRoute()}/redact-event";
        await _client.PerformCommand(route, payload);
    }

    /// <inheritdoc/>
    public async Task Redact(EventSourceId eventSourceId, RedactionReason? reason = default, params Type[] eventTypes)
    {
        reason ??= RedactionReason.Unknown;
        var eventTypeIds = eventTypes.Select(_ => _eventTypes.GetEventTypeFor(_).Id).ToArray();
        var payload = new RedactEvents(eventSourceId, reason, eventTypeIds);
        var route = $"{GetBaseRoute()}/redact-events";
        await _client.PerformCommand(route, payload);
    }

    string GetBaseRoute() => $"/api/events/store/{_executionContextManager.Current.MicroserviceId}/{_executionContextManager.Current.TenantId}/sequence/{_eventSequenceId}";
}
