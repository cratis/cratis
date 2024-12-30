// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using Cratis.Chronicle.Contracts;
using Cratis.Chronicle.Storage;

namespace Cratis.Chronicle.Services;

/// <summary>.
/// Represents an implementation of <see cref="IEventStores"/>.
/// </summary>
/// <param name="storage">The <see cref="IStorage"/> for working with the storage.</param>
public class EventStores(IStorage storage) : IEventStores
{
    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetEventStores()
    {
        var eventStores = await storage.GetEventStores();
        return eventStores.Select(_ => _.Value).ToArray();
    }

    /// <inheritdoc/>
    public IObservable<IEnumerable<string>> ObserveEventStores()
    {
        var subject = new Subject<IEnumerable<string>>();
        storage.ObserveEventStores().Subscribe(eventStores => subject.OnNext(eventStores.Select(_ => _.Value).ToArray()));
        return subject;
    }
}
