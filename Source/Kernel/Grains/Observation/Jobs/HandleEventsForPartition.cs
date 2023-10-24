// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Kernel.Grains.Jobs;
using Aksio.Cratis.Observation;
using Aksio.DependencyInversion;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.SyncWork;

namespace Aksio.Cratis.Kernel.Grains.Observation.Jobs;

/// <summary>
/// Represents a step in a replay job.
/// </summary>
public class HandleEventsForPartition : JobStep<HandleEventsForPartitionArguments, HandleEventsForPartitionState>, IHandleEventsForPartition
{
    const string SubscriberDisconnected = "Subscriber is disconnected";

    readonly ProviderFor<IEventSequenceStorage> _eventSequenceStorageProvider;
    readonly IExecutionContextManager _executionContextManager;
    IObserver? _observer;
    IObserverSubscriber? _subscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandleEventsForPartition"/> class.
    /// </summary>
    /// <param name="state"><see cref="IPersistentState{TState}"/> for managing state of the job step.</param>
    /// <param name="eventSequenceStorageProvider">Provider for <see cref="IEventSequenceStorage"/>.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with the execution context.</param>
    /// <param name="taskScheduler"><see cref="LimitedConcurrencyLevelTaskScheduler"/> to use for scheduling.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public HandleEventsForPartition(
        [PersistentState(nameof(JobStepState), WellKnownGrainStorageProviders.JobSteps)] IPersistentState<HandleEventsForPartitionState> state,
        ProviderFor<IEventSequenceStorage> eventSequenceStorageProvider,
        IExecutionContextManager executionContextManager,
        LimitedConcurrencyLevelTaskScheduler taskScheduler,
        ILogger<HandleEventsForPartition> logger) : base(state, taskScheduler, logger)
    {
        _eventSequenceStorageProvider = eventSequenceStorageProvider;
        _executionContextManager = executionContextManager;
    }

    /// <inheritdoc/>
    protected override Task PrepareStep(HandleEventsForPartitionArguments request)
    {
        var eventSourceId = (EventSourceId)(request.Partition.Value.ToString() ?? string.Empty);
        _observer = GrainFactory.GetGrain<IObserver>(request.ObserverId, request.ObserverKey);

        State.NextEventSequenceNumber = request.StartEventSequenceNumber;

        if (!request.ObserverSubscription.IsSubscribed)
        {
            return Task.CompletedTask;
        }

        var key = new ObserverSubscriberKey(
            request.ObserverKey.MicroserviceId,
            request.ObserverKey.TenantId,
            request.ObserverKey.EventSequenceId,
            eventSourceId,
            request.ObserverKey.SourceMicroserviceId,
            request.ObserverKey.SourceTenantId);

        _subscriber = (GrainFactory.GetGrain(request.ObserverSubscription.SubscriberType, request.ObserverSubscription.ObserverId, key) as IObserverSubscriber)!;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task<JobStepResult> PerformStep(HandleEventsForPartitionArguments request)
    {
        if (_subscriber == null || !request.ObserverSubscription.IsSubscribed)
        {
            return JobStepResult.Failed(SubscriberDisconnected);
        }

        var eventSourceId = (EventSourceId)(request.Partition.Value.ToString() ?? string.Empty);
        _executionContextManager.Establish(request.ObserverKey.TenantId, CorrelationId.New(), request.ObserverKey.MicroserviceId);
        var eventSequenceStorage = _eventSequenceStorageProvider();
        var events = await eventSequenceStorage.GetFromSequenceNumber(
            request.ObserverKey.EventSequenceId,
            request.StartEventSequenceNumber,
            eventSourceId,
            request.EventTypes);

        var subscriberContext = new ObserverSubscriberContext(request.ObserverSubscription.Arguments);

        var failed = false;
        var exceptionMessages = Enumerable.Empty<string>();
        var exceptionStackTrace = string.Empty;
        var tailEventSequenceNumber = EventSequenceNumber.Unavailable;

        while (await events.MoveNext())
        {
            try
            {
                if (!events.Current.Any())
                {
                    break;
                }

                tailEventSequenceNumber = events.Current.First().Metadata.SequenceNumber;
                var result = await _subscriber!.OnNext(events.Current, subscriberContext);
                switch (result.State)
                {
                    case ObserverSubscriberState.Failed:
                        failed = true;
                        exceptionMessages = result.ExceptionMessages;
                        exceptionStackTrace = result.ExceptionStackTrace;
                        if (result.LastSuccessfulObservation != EventSequenceNumber.Unavailable)
                        {
                            tailEventSequenceNumber = result.LastSuccessfulObservation;
                        }
                        break;
                    case ObserverSubscriberState.Disconnected:
                        failed = true;
                        exceptionMessages = new[] { SubscriberDisconnected };
                        break;
                }
            }
            catch (Exception ex)
            {
                failed = true;
                exceptionMessages = ex.GetAllMessages().ToArray();
                exceptionStackTrace = ex.StackTrace ?? string.Empty;
            }

            if (failed)
            {
                await _observer!.PartitionFailed(eventSourceId, tailEventSequenceNumber, exceptionMessages, exceptionStackTrace);
                return new JobStepResult(JobStepStatus.Failed, exceptionMessages, exceptionStackTrace);
            }
        }

        return JobStepResult.Succeeded;
    }
}