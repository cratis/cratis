// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Projections.Definitions;

namespace Cratis.Chronicle.Projections;

/// <summary>
/// Defines the builder for building out a <see cref="IProjectionFor{TModel}"/>.
/// </summary>
/// <typeparam name="TModel">Type of model.</typeparam>
public interface IProjectionBuilderFor<TModel> : IProjectionBuilder<TModel, IProjectionBuilderFor<TModel>>
{
    /// <summary>
    /// Names the projection. Default behavior is to use the type of the models full name.
    /// </summary>
    /// <param name="name">The name of the projection to use.</param>
    /// <returns>Builder continuation.</returns>
    IProjectionBuilderFor<TModel> WithName(string name);

    /// <summary>
    /// Names the model - typically used by storage as name of storage unit (collection, table etc.)
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <returns>Builder continuation.</returns>
    IProjectionBuilderFor<TModel> ModelName(string modelName);

    /// <summary>
    /// Set the projection to not be rewindable - its a moving forward only projection.
    /// </summary>
    /// <returns>Builder continuation.</returns>
    IProjectionBuilderFor<TModel> NotRewindable();

    /// <summary>
    /// Build a <see cref="ProjectionDefinition"/>.
    /// </summary>
    /// <returns>A new <see cref="ProjectionDefinition"/>.</returns>
    ProjectionDefinition Build();
}
