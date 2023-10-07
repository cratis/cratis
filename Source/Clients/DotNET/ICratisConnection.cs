// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Connections;
using Aksio.Cratis.Kernel.Contracts.EventSequences;

namespace Aksio.Cratis;

/// <summary>
/// Defines a system that manages the connection to Cratis.
/// </summary>
public interface ICratisConnection : IDisposable
{
    /// <summary>
    /// Gets the <see cref="IConnectionLifecycle"/> service.
    /// </summary>
    IConnectionLifecycle Lifecycle { get; }

    /// <summary>
    /// Gets the <see cref="IEventSequences"/> service.
    /// </summary>
    IEventSequences EventSequences { get; }
}
