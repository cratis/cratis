// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Concepts.Configuration;

/// <summary>
/// Represents the Chronicle options.
/// </summary>
public class ChronicleOptions
{
    /// <summary>
    /// Port to listen on for gRPC.
    /// </summary>
    public int Port { get; init; } = 35000;

    /// <summary>
    /// Gets the port for the REST API.
    /// </summary>
    public int ApiPort { get; init; } = 8080;

    /// <summary>
    /// Gets or inits the storage configuration.
    /// </summary>
    public Storage Storage { get; init; } = new Storage();
}