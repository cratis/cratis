// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Aggregates;

namespace Cratis.Chronicle;

/// <summary>
/// Delegate for providing <see cref="IAggregateRootFactory"/> instances.
/// </summary>
/// <param name="eventStore">The <see cref="IEventStore"/> the factory is for.</param>
/// <returns><see cref="IAggregateRootFactory"/> instance.</returns>
public delegate IAggregateRootFactory AggregateRootFactoryProvider(IEventStore eventStore);
