// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Orleans.Aggregates;

/// <summary>
/// Exception that gets thrown when the type is not an <see cref="IAggregateRoot"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="WrongTypeForAggregateRoot"/>.
/// </remarks>
/// <param name="type">The type that is violating.</param>
public class WrongTypeForAggregateRoot(Type type)
    : Exception($"Type '{type.FullName}' is wrong type, expected to extend ${typeof(IAggregateRoot).FullName}")
{
    /// <summary>
    /// Throw if the type is not an <see cref="IAggregateRoot"/>.
    /// </summary>
    /// <param name="type">Type to validate.</param>
    /// <exception cref="WrongTypeForAggregateRoot">Thrown when the type is not an <see cref="IAggregateRoot"/>.</exception>
    public static void ThrowIfWrongType(Type type)
    {
        if (!type.IsAssignableTo(typeof(IAggregateRoot)))
        {
            throw new WrongTypeForAggregateRoot(type);
        }
    }
}
