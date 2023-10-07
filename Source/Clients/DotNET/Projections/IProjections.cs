// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Aksio.Cratis.Kernel.Contracts.Projections;

namespace Aksio.Cratis.Projections;

/// <summary>
/// Defines a system that holds all <see cref="ProjectionDefinition">projection definitions</see> defined through <see cref="IProjectionBuilderFor{T}"/>.
/// </summary>
public interface IProjections
{
    /// <summary>
    /// Gets all the <see cref="ProjectionDefinition">projection definitions</see>.
    /// </summary>
    IImmutableList<ProjectionDefinition> Definitions { get; }

    /// <summary>
    /// Discover and register all reducers discovered from the entry assembly.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task RegisterKnownProjections();
}
