// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Keys;
using Aksio.Cratis.Kernel.Observation;
using MongoDB.Driver;

namespace Aksio.Cratis.Kernel.MongoDB.Observation.Indexes;

/// <summary>
/// Represents an implementation of <see cref="IObserverKeys"/> for MongoDB.
/// </summary>
public class MongoDBObserverKeys : IObserverKeys
{
    readonly IMongoCollection<Event> _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBObserverKeys"/> class.
    /// </summary>
    /// <param name="collection">The <see cref="IMongoCollection{T}"/> that holds the keys.</param>
    public MongoDBObserverKeys(IMongoCollection<Event> collection)
    {
        _collection = collection;
    }

    /// <inheritdoc/>
    public IAsyncEnumerator<Key> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var cursor = _collection.Distinct(_ => _.EventSourceId, _ => true, cancellationToken: cancellationToken);
        return new MongoDBObserverKeysAsyncEnumerator(null!);
    }
}