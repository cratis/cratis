// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;

namespace Cratis.Chronicle.Reducers;

/// <summary>
/// Represents the result of a reduce operation.
/// </summary>
/// <param name="State">Potential state, unless errored.</param>
/// <param name="LastSuccessfullyObservedEvent">The sequence number of the last successfully observed event.</param>
/// <param name="ErrorMessages">Collection of error messages, if any.</param>
/// <param name="StackTrace">The stack trace, if an error occurred.</param>
public record InternalReduceResult(
    object? State,
    EventSequenceNumber LastSuccessfullyObservedEvent,
    IEnumerable<string> ErrorMessages,
    string StackTrace)
{
    /// <summary>
    /// Gets whether or not the reduce operation was successful.
    /// </summary>
    public bool IsSuccess => !ErrorMessages.Any();
}
