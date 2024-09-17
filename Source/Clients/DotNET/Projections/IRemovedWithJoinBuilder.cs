// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Contracts.Projections;

namespace Cratis.Chronicle.Projections;

/// <summary>
/// Defines a builder for building out removed with join statements.
/// </summary>
/// <typeparam name="TModel">Model to build for.</typeparam>
/// <typeparam name="TEvent">Event to build for.</typeparam>
/// <typeparam name="TBuilder">Type of actual builder.</typeparam>
public interface IRemovedWithJoinBuilder<TModel, TEvent, TBuilder>
    where TBuilder : class
{
    /// <summary>
    /// Build the removed with join definition.
    /// </summary>
    /// <returns>A new <see cref="RemovedWithJoinDefinition"/>.</returns>
    RemovedWithJoinDefinition Build();
}