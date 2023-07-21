// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Connections;
using Aksio.Cratis.Reducers;

namespace Aksio.Cratis.Kernel.Grains.Observation.Reducers.Clients;

/// <summary>
/// Defines a grain for working with all <see cref="IClientReducer">client reducers</see>.
/// </summary>
public interface IClientReducers : IGrainWithGuidKey
{
    /// <summary>
    /// Register a collection of client reducers.
    /// </summary>
    /// <param name="connectionId"><see cref="ConnectionId"/> to register with.</param>
    /// <param name="registrations">Collection of <see cref="ClientReducerRegistration"/>.</param>
    /// <returns>Awaitable task.</returns>
    Task Register(ConnectionId connectionId, IEnumerable<ClientReducerRegistration> registrations);
}
