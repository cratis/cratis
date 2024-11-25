// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Concepts.Events;
using Cratis.Chronicle.Concepts.Keys;
using Cratis.Chronicle.Concepts.Observation;
using Microsoft.Extensions.Logging;

namespace Cratis.Chronicle.Grains.Observation;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type

internal static partial class ObserverLogMessages
{
    [LoggerMessage(LogLevel.Debug, "Subscribing observer")]
    internal static partial void Subscribing(this ILogger<Observer> logger);

    [LoggerMessage(LogLevel.Warning, "Partition {Partition} failed for event with sequence number {EventSequenceNumber}")]
    internal static partial void PartitionFailed(this ILogger<Observer> logger, Key partition, EventSequenceNumber eventSequenceNumber);

    [LoggerMessage(LogLevel.Debug, "Trying to recover partition {Partition}")]
    internal static partial void TryingToRecoverFailedPartition(this ILogger<Observer> logger, Key partition);

    [LoggerMessage(LogLevel.Warning, "Giving up on trying to recover failed partition {Partition} automatically")]
    internal static partial void GivingUpOnRecoveringFailedPartition(this ILogger<Observer> logger, Key partition);

    [LoggerMessage(LogLevel.Debug, "Attempting to replay partition {Partition} to event sequence number {ToEventSequenceNumber}")]
    internal static partial void AttemptReplayPartition(this ILogger<Observer> logger, Key partition, EventSequenceNumber toEventSequenceNumber);

    [LoggerMessage(LogLevel.Debug, "Finished replay for partition {Partition}")]
    internal static partial void FinishedReplayForPartition(this ILogger<Observer> logger, Key partition);

    [LoggerMessage(LogLevel.Debug, "Partition {Partition} is replaying events and cannot accept new events to handle")]
    internal static partial void PartitionReplayingCannotHandleNewEvents(this ILogger<Observer> logger, Key partition);
}

internal static class ObserverScopes
{
    internal static IDisposable? BeginObserverScope(this ILogger<Observer> logger, ObserverId observerId, ObserverKey observerKey) =>
        logger.BeginScope(new Dictionary<string, object>
        {
            ["ObserverId"] = observerId,
            ["EventStore"] = observerKey.EventStore,
            ["Namespace"] = observerKey.Namespace,
            ["EventSequenceId"] = observerKey.EventSequenceId
        });
}
