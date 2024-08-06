// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Concepts.Events;
using Cratis.Chronicle.Concepts.EventSequences;
using Cratis.Chronicle.Storage.Events.Constraints;
using MongoDB.Driver;

namespace Cratis.Chronicle.Storage.MongoDB.Events.Constraints;

/// <summary>
/// Represents an implementation of <see cref="IUniqueConstraintsStorage"/>.
/// </summary>
/// <param name="database">The <see cref="IEventStoreNamespaceDatabase"/> to use.</param>
/// <param name="eventSequenceId">The <see cref="EventSequenceId"/> to use.</param>
public class UniqueEventTypesConstraintsStorage(IEventStoreNamespaceDatabase database, EventSequenceId eventSequenceId) : IUniqueEventTypesConstraintsStorage
{
    readonly IMongoCollection<Event> _collection = database.GetEventSequenceCollectionFor(eventSequenceId);

    /// <inheritdoc/>
    public async Task<(bool Exists, EventSequenceNumber SequenceNumber)> Exists(EventType eventType, EventSourceId eventSourceId)
    {
        var filter = Builders<Event>.Filter.Eq(_ => _.Type, eventType.Id);
        filter &= Builders<Event>.Filter.Eq(_ => _.EventSourceId, eventSourceId);

        var result = await _collection.FindAsync(filter);
        var existing = result.FirstOrDefault();
        if (existing is not null)
        {
            return (true, existing.SequenceNumber);
        }

        return (false, EventSequenceNumber.Unavailable);
    }
}
