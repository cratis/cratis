// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Nodes;
using Cratis.Chronicle.Auditing;
using Cratis.Chronicle.Contracts.EventSequences;
using Cratis.Chronicle.Events;
using Cratis.Chronicle.EventSequences;
using Cratis.Chronicle.Identities;
using Cratis.Chronicle.Storage;
using ProtoBuf.Grpc;

namespace Cratis.Chronicle.Services.EventSequences;

/// <summary>
/// Represents an implementation of <see cref="IEventSequences"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EventSequences"/> class.
/// </remarks>
/// <param name="grainFactory"><see cref="IGrainFactory"/> to get grains with.</param>
/// <param name="storage"><see cref="IStorage"/> for storing events.</param>
/// <param name="jsonSerializerOptions"><see cref="JsonSerializerOptions"/> for serialization.</param>
public class EventSequences(
    IGrainFactory grainFactory,
    IStorage storage,
    JsonSerializerOptions jsonSerializerOptions) : IEventSequences
{
    /// <inheritdoc/>
    public async Task<AppendResponse> Append(AppendRequest request, CallContext context = default)
    {
        var eventSequence = GetEventSequence(request.EventStoreName, request.Namespace, request.EventSequenceId);
        await eventSequence.Append(
            request.EventSourceId,
            request.EventType.ToChronicle(),
            JsonSerializer.Deserialize<JsonNode>(request.Content, jsonSerializerOptions)!.AsObject(),
            request.Causation.ToChronicle(),
            request.CausedBy.ToChronicle());

        return new AppendResponse();
    }

    /// <inheritdoc/>
    public async Task<AppendManyResponse> AppendMany(AppendManyRequest request, CallContext context = default)
    {
        var eventSequence = GetEventSequence(request.EventStoreName, request.Namespace, request.EventSequenceId);
        await eventSequence.AppendMany(
            request.EventSourceId,
            request.Events.ToChronicle(),
            request.Causation.ToChronicle(),
            request.CausedBy.ToChronicle());

        return new AppendManyResponse();
    }

    /// <inheritdoc/>
    public async Task<GetForEventSourceIdAndEventTypesResponse> GetForEventSourceIdAndEventTypes(GetForEventSourceIdAndEventTypesRequest request, CallContext context = default)
    {
        var eventSequence = storage
            .GetEventStore(request.EventStoreName)
            .GetNamespace(request.Namespace)
           .GetEventSequence(request.EventSequenceId);

        var cursor = await eventSequence.GetFromSequenceNumber(
            EventSequenceNumber.First,
            request.EventSourceId,
            request.EventTypes.ToChronicle());

        var events = new List<Contracts.Events.AppendedEvent>();
        while (await cursor.MoveNext())
        {
            var current = cursor.Current;
            events.AddRange(current.ToContract());
        }
        return new()
        {
            Events = events
        };
    }

    Grains.EventSequences.IEventSequence GetEventSequence(EventStoreName eventStore, EventStoreNamespaceName @namespace, EventSequenceId eventSequenceId) =>
        grainFactory.GetGrain<Grains.EventSequences.IEventSequence>(new EventSequenceKey(eventSequenceId, eventStore, @namespace));
}
