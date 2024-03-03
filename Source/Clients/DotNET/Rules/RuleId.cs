// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Projections;

namespace Cratis.Rules;

/// <summary>
/// Represents the unique identifier of a projection.
/// </summary>
/// <param name="Value">The value.</param>
public record RuleId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Implicitly convert from <see cref="Guid"/> to <see cref="ProjectionId"/>.
    /// </summary>
    /// <param name="value"><see cref="Guid"/> to convert from.</param>
    public static implicit operator RuleId(Guid value) => new(value);

    /// <summary>
    /// Implicitly convert from string representation of a <see cref="Guid"/> to <see cref="ProjectionId"/>.
    /// </summary>
    /// <param name="value"><see cref="Guid"/> to convert from.</param>
    public static implicit operator RuleId(string value) => new(Guid.Parse(value));
}
