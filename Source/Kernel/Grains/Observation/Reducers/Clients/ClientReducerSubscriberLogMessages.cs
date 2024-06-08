// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Observation.Reducers;
using Cratis.Chronicle.Events;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Grains.Observation.Reducers.Clients;

internal static partial class ClientReducerSubscriberLogMessages
{
    [LoggerMessage(0, LogLevel.Trace, "Reducer {ReducerId} in microservice {MicroserviceId} for tenant {TenantId} received event of type {EventTypeId} in sequence {EventSequenceId} with sequence number {EventSequenceNumber}")]
    internal static partial void EventReceived(this ILogger<ClientReducerSubscriber> logger, ReducerId reducerId, MicroserviceId microserviceId, TenantId tenantId, EventTypeId eventTypeId, EventSequenceId eventSequenceId, EventSequenceNumber eventSequenceNumber);
}
