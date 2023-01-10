// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.Connections;

/// <summary>
/// Represents the information related to a connected client.
/// </summary>
/// <param name="ConnectionId">The unique connection id.</param>
/// <param name="ClientUri">The uri of the client.</param>
/// <param name="Version">Version of the client.</param>
public record ConnectedClient(ConnectionId ConnectionId, string ClientUri, string Version);
