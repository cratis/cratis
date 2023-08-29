// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Keys;

namespace Aksio.Cratis.Kernel.Observation;

/// <summary>
/// Defines a system for indexing keys for an observer.
/// </summary>
public interface IObserverKeyIndex
{
    /// <summary>
    /// Get the keys for a specific microservice and tenant.
    /// </summary>
    /// <returns>All the <see cref="IObserverKeys"/>.</returns>
    Task<IObserverKeys> GetKeysFor();

    /// <summary>
    /// Add a key to the index.
    /// </summary>
    /// <param name="key"><see cref="Key"/> to add.</param>
    /// <returns>Awaitable task.</returns>
    Task Add(Key key);

    /// <summary>
    /// Rebuild the index.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Rebuild();
}