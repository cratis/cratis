// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Aksio.Cratis.Events;
using Aksio.Cratis.Jobs;
using Aksio.Cratis.Kernel.Grains.EventSequences;
using Aksio.Cratis.Kernel.Grains.Jobs;
using Aksio.Cratis.Kernel.Grains.Observation.Jobs;
using Aksio.Cratis.Kernel.Grains.Observation.States;
using Aksio.Cratis.Kernel.Keys;
using Aksio.Cratis.Kernel.Observation;
using Aksio.Cratis.Kernel.Orleans.StateMachines;
using Aksio.Cratis.Kernel.Storage.EventSequences;
using Aksio.Cratis.Kernel.Storage.Observation;
using Aksio.Cratis.Metrics;
using Aksio.Cratis.Observation;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

namespace Aksio.Cratis.Kernel.Grains.Observation;

/// <summary>
/// Represents an implementation of <see cref="IObserver"/>.
/// </summary>
[StorageProvider(ProviderName = WellKnownGrainStorageProviders.Observers)]
public class Observer : StateMachine<ObserverState>, IObserver, IRemindable
{
    readonly ILogger<Observer> _logger;
    readonly IMeter<Observer> _meter;
    readonly ILoggerFactory _loggerFactory;
    readonly IPersistentState<FailedPartitions> _failuresState;
    readonly IObserverServiceClient _replayStateServiceClient;
    IStreamProvider _streamProvider = null!;
    ObserverId _observerId = Guid.Empty;
    ObserverKey _observerKey = ObserverKey.NotSet;
    ObserverSubscription _subscription;
    IJobsManager _jobsManager = null!;
    bool _stateWritingSuspended;
    IEventSequence _eventSequence = null!;
    IMeterScope<Observer>? _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="Observer"/> class.
    /// </summary>
    /// <param name="failures"><see cref="IPersistentState{T}"/> for failed partitions.</param>
    /// <param name="replayStateServiceClient"><see cref="IObserverServiceClient"/> for notifying about replay to all silos.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    /// <param name="meter"><see cref="Meter{T}"/> for the observer.</param>
    /// <param name="loggerFactory"><see cref="ILoggerFactory"/> for creating loggers.</param>
    public Observer(
        [PersistentState(nameof(FailedPartition), WellKnownGrainStorageProviders.FailedPartitions)]
        IPersistentState<FailedPartitions> failures,
        IObserverServiceClient replayStateServiceClient,
        ILogger<Observer> logger,
        IMeter<Observer> meter,
        ILoggerFactory loggerFactory)
    {
        _failuresState = failures;
        _replayStateServiceClient = replayStateServiceClient;
        _logger = logger;
        _meter = meter;
        _loggerFactory = loggerFactory;
        _subscription = ObserverSubscription.Unsubscribed;
    }

    /// <inheritdoc/>
    protected override Type InitialState => typeof(Routing);

    FailedPartitions Failures => _failuresState.State;

    /// <inheritdoc/>
    public override async Task OnActivation(CancellationToken cancellationToken)
    {
        // Keep the Grain alive forever: Confirmed here: https://github.com/dotnet/orleans/issues/1721#issuecomment-216566448
        DelayDeactivation(TimeSpan.MaxValue);

        _observerId = this.GetPrimaryKey(out var keyAsString);
        _observerKey = ObserverKey.Parse(keyAsString);

        _streamProvider = this.GetStreamProvider(WellKnownProviders.EventSequenceStreamProvider);
        _jobsManager = GrainFactory.GetGrain<IJobsManager>(0, new JobsManagerKey(_observerKey.MicroserviceId, _observerKey.TenantId));

        await _failuresState.ReadStateAsync();

        _eventSequence = GrainFactory.GetGrain<IEventSequence>(
            _observerKey.EventSequenceId,
            new EventSequenceKey(_observerKey.MicroserviceId, _observerKey.TenantId));

        _metrics = _meter.BeginObserverScope(_observerId, _observerKey);
    }

    /// <inheritdoc/>
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await TransitionTo<Disconnected>();
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    /// <inheritdoc/>
    public Task Ensure() => Task.CompletedTask;

#pragma warning disable CA1721 // Property names should not match get methods
    /// <inheritdoc/>
    public Task<ObserverState> GetState() => Task.FromResult(State);
#pragma warning restore CA1721 // Property names should not match get methods

    /// <inheritdoc/>
    public Task SetHandledStats(EventCount handled, EventSequenceNumber lastHandledEventSequenceNumber)
    {
        State = State with
        {
            Handled = handled,
            LastHandledEventSequenceNumber = lastHandledEventSequenceNumber
        };

        return WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task SetNameAndType(ObserverName name, ObserverType type)
    {
        State = State with { Name = name, Type = type };
        await WriteStateAsync();
    }

    /// <inheritdoc/>
    public Task<ObserverSubscription> GetSubscription() => Task.FromResult(_subscription);

    /// <inheritdoc/>
    public Task<bool> IsSubscribed() => Task.FromResult(_subscription.IsSubscribed);

    /// <inheritdoc/>
    public Task<IEnumerable<EventType>> GetEventTypes() => Task.FromResult(State.EventTypes);

    /// <inheritdoc/>
    public async Task Subscribe<TObserverSubscriber>(IEnumerable<EventType> eventTypes, object? subscriberArgs = null)
        where TObserverSubscriber : IObserverSubscriber
    {
        using var scope = _logger.BeginObserverScope(_observerId, _observerKey);

        _logger.Subscribing();

        _subscription = new ObserverSubscription(
            _observerId,
            _observerKey,
            eventTypes,
            typeof(TObserverSubscriber),
            subscriberArgs);

        await TransitionTo<Routing>();
        await TryRecoverAllFailedPartitions();
    }

    /// <inheritdoc/>
    public override IImmutableList<IState<ObserverState>> CreateStates() => new IState<ObserverState>[]
    {
        new Disconnected(),

        new Routing(
            _observerKey,
            new ReplayEvaluator(
                GrainFactory,
                _observerKey.MicroserviceId,
                _observerKey.TenantId),
            _eventSequence,
            _loggerFactory.CreateLogger<Routing>()),

        new CatchUp(
            _observerId,
            _observerKey,
            _jobsManager,
            _loggerFactory.CreateLogger<CatchUp>()),

        new ResumeReplay(
            _observerId,
            _observerKey,
            _replayStateServiceClient,
            _jobsManager),

        new Replay(
            _observerId,
            _observerKey,
            _replayStateServiceClient,
            _jobsManager,
            _loggerFactory.CreateLogger<Replay>()),

        new Indexing(),

        new Observing(
            _streamProvider,
            _observerKey.MicroserviceId,
            _observerKey.TenantId,
            _observerKey.EventSequenceId,
            _loggerFactory.CreateLogger<Observing>())
    }.ToImmutableList();

    /// <inheritdoc/>
    public async Task Unsubscribe()
    {
        _subscription = ObserverSubscription.Unsubscribed;
        await TransitionTo<Disconnected>();
    }

    /// <inheritdoc/>
    public async Task Replay()
    {
        if (State.RunningState == ObserverRunningState.Active)
        {
            await TransitionTo<Replay>();
        }
    }

    /// <inheritdoc/>
    public async Task ReplayPartition(Key partition)
    {
        await _jobsManager.Start<IReplayObserverPartition, ReplayObserverPartitionRequest>(
            JobId.New(),
            new(
                _observerId,
                _observerKey,
                _subscription,
                partition,
                EventSequenceNumber.First,
                EventSequenceNumber.Max,
                State.EventTypes));
    }

    /// <inheritdoc/>
    public async Task ReplayPartitionTo(Key partition, EventSequenceNumber sequenceNumber)
    {
        await _jobsManager.Start<IReplayObserverPartition, ReplayObserverPartitionRequest>(
            JobId.New(),
            new(
                _observerId,
                _observerKey,
                _subscription,
                partition,
                EventSequenceNumber.First,
                sequenceNumber,
                State.EventTypes));
    }

    /// <inheritdoc/>
    public Task PartitionReplayed(Key partition) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task PartitionFailed(
        Key partition,
        EventSequenceNumber sequenceNumber,
        IEnumerable<string> exceptionMessages,
        string exceptionStackTrace)
    {
        using var scope = _logger.BeginObserverScope(_observerId, _observerKey);

        _metrics?.PartitionFailed(partition);

        _logger.PartitionFailed(
            partition,
            sequenceNumber);

        var failure = Failures.RegisterAttempt(partition, sequenceNumber, exceptionMessages, exceptionStackTrace);
        var time = TimeSpan.FromSeconds((failure.Attempts.Count() - 1) * 2);
        if (time.TotalMilliseconds == 0)
        {
            time = TimeSpan.FromMilliseconds(100);
        }

        if (failure.Attempts.Count() < 10)
        {
            await this.RegisterOrUpdateReminder(partition.ToString(), time, TimeSpan.FromHours(48));
        }
        else
        {
            _logger.GivingUpOnRecoveringFailedPartition(partition);
        }

        await _failuresState.WriteStateAsync();
    }

    /// <inheritdoc/>
    public Task FailedPartitionRecovered(Key partition)
    {
        Failures.Remove(partition);
        return _failuresState.WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task TryRecoverFailedPartition(Key partition)
    {
        using var scope = _logger.BeginObserverScope(_observerId, _observerKey);

        _logger.TryingToRecoverFailedPartition(partition);

        var failure = Failures.Get(partition);
        if (failure is null) return;
        var lastAttempt = failure.LastAttempt;
        if (lastAttempt is null) return;

        await RemoveReminder(partition.ToString());

        await _jobsManager.Start<IRetryFailedPartitionJob, RetryFailedPartitionRequest>(
            JobId.New(),
            new(
                _observerId,
                _observerKey,
                _subscription,
                partition,
                lastAttempt.SequenceNumber,
                State.EventTypes));
    }

    /// <inheritdoc/>
    public async Task TryRecoverAllFailedPartitions()
    {
        foreach (var partition in Failures.Partitions)
        {
            await TryRecoverFailedPartition(partition.Partition);
        }
    }

    /// <inheritdoc/>
    public async Task Handle(Key partition, IEnumerable<AppendedEvent> events)
    {
        if (!_subscription.IsSubscribed || Failures.IsFailed(partition))
        {
            return;
        }

        var shouldHandle = events.Any(_ => _subscription.EventTypes.Contains(_.Metadata.Type));
        if (!shouldHandle)
        {
            State = State with { NextEventSequenceNumber = events.Last().Metadata.SequenceNumber.Next() };
            await WriteStateAsync();
            return;
        }

        var failed = false;
        var exceptionMessages = Enumerable.Empty<string>();
        var exceptionStackTrace = string.Empty;
        var tailEventSequenceNumber = State.NextEventSequenceNumber;

        events = events.Where(_ => _.Metadata.SequenceNumber >= State.NextEventSequenceNumber).ToArray();
        var handledCount = EventCount.Zero;
        if (events.Any())
        {
            using (new WriteSuspension(this))
            {
                try
                {
                    var key = new ObserverSubscriberKey(
                        _observerKey.MicroserviceId,
                        _observerKey.TenantId,
                        _observerKey.EventSequenceId,
                        partition,
                        _observerKey.SourceMicroserviceId,
                        _observerKey.SourceTenantId);

                    var firstEvent = events.First();
                    var lastEvent = events.Last();

                    var subscriber = (GrainFactory.GetGrain(_subscription.SubscriberType, _observerId, key) as IObserverSubscriber)!;
                    tailEventSequenceNumber = firstEvent.Metadata.SequenceNumber;
                    var result = await subscriber.OnNext(events, new(_subscription.Arguments));
                    if (result.LastSuccessfulObservation != EventSequenceNumber.Unavailable)
                    {
                        handledCount = events.Count(_ => _.Metadata.SequenceNumber <= result.LastSuccessfulObservation);
                    }
                    else
                    {
                        handledCount = EventCount.Zero;
                    }
                    if (result.State == ObserverSubscriberState.Failed)
                    {
                        failed = true;
                        exceptionMessages = result.ExceptionMessages;
                        exceptionStackTrace = result.ExceptionStackTrace;
                        tailEventSequenceNumber = result.LastSuccessfulObservation == EventSequenceNumber.Unavailable ?
                            firstEvent.Metadata.SequenceNumber :
                            result.LastSuccessfulObservation;
                    }
                    else if (result.State == ObserverSubscriberState.Disconnected)
                    {
                        await Unsubscribe();
                        if (result.LastSuccessfulObservation == EventSequenceNumber.Unavailable)
                        {
                            return;
                        }
                    }

                    State = State with { NextEventSequenceNumber = result.LastSuccessfulObservation.Next() };
                    if (!State.LastHandledEventSequenceNumber.IsActualValue || State.LastHandledEventSequenceNumber < result.LastSuccessfulObservation)
                    {
                        State = State with
                        {
                            LastHandledEventSequenceNumber = result.LastSuccessfulObservation,
                            Handled = State.Handled + handledCount
                        };
                    }
                }
                catch (Exception ex)
                {
                    failed = true;
                    exceptionMessages = ex.GetAllMessages().ToArray();
                    exceptionStackTrace = ex.StackTrace ?? string.Empty;
                }
            }

            if (failed)
            {
                await PartitionFailed(partition, tailEventSequenceNumber, exceptionMessages, exceptionStackTrace);
            }

            await WriteStateAsync();
        }
    }

    /// <inheritdoc/>
    public async Task ReportHandledEvents(EventCount count)
    {
        State = State with { Handled = State.Handled + count };
        await WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        await RemoveReminder(reminderName);

        var partition = _failuresState.State.Partitions.FirstOrDefault(_ => _.Partition.ToString() == reminderName);
        if (partition is not null)
        {
            await TryRecoverFailedPartition(partition.Partition);
        }
    }

    /// <summary>
    /// Set subscription explicitly, without subscribing. This method is internal and visible to the test suite and only meant to be used with testing.
    /// </summary>
    /// <param name="subscription">Subscription to set.</param>
    internal void SetSubscription(ObserverSubscription subscription)
    {
        _subscription = subscription;
    }

    /// <inheritdoc/>
    protected override Task OnBeforeEnteringState(IState<ObserverState> state)
    {
        if (state is BaseObserverState observerState)
        {
            State = State with { RunningState = observerState.RunningState };
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task WriteStateAsync()
    {
        if (_stateWritingSuspended) return;
        await base.WriteStateAsync();
    }

    async Task RemoveReminder(string reminderName)
    {
        var reminder = await this.GetReminder(reminderName);
        if (reminder is not null)
        {
            await this.UnregisterReminder(reminder);
        }
    }

    class WriteSuspension : IDisposable
    {
        readonly Observer _observer;

        public WriteSuspension(Observer observer)
        {
            _observer = observer;
            _observer._stateWritingSuspended = true;
        }

        public void Dispose() => _observer._stateWritingSuspended = false;
    }
}
