// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Events.Constraints;

/// <summary>
/// Represents methods for converting between <see cref="Contracts.Events.Constraints.ConstraintViolation"/> and <see cref="ConstraintViolation"/>.
/// </summary>
public static class ConstraintViolationConverters
{
    /// <summary>
    /// Convert a <see cref="Contracts.Events.Constraints.ConstraintViolation"/> to a <see cref="ConstraintViolation"/>.
    /// </summary>
    /// <param name="violation"><see cref="ConstraintViolation"/> to convert from.</param>
    /// <returns>Converted <see cref="Contracts.Events.Constraints.ConstraintViolation"/>.</returns>
    public static ConstraintViolation ToClient(this Contracts.Events.Constraints.ConstraintViolation violation) =>
        new(
            violation.EventType.ToClient(),
            violation.SequenceNumber,
            violation.ConstraintName,
            violation.Message,
            new(violation.Details));
}
