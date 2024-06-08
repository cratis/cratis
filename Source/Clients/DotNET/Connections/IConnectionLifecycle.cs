// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Connections;

namespace Cratis.Chronicle.Connections;

/// <summary>
/// Defines a system for the lifecycle of the client.
/// </summary>
public interface IConnectionLifecycle
{
    /// <summary>
    /// Gets whether or not the client is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the current connection identifier.
    /// </summary>
    ConnectionId ConnectionId { get; }

    /// <summary>
    /// Called when the client gets connected to the kernel.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Connected();

    /// <summary>
    /// Called when the client is disconnected to the kernel.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Disconnected();
}
