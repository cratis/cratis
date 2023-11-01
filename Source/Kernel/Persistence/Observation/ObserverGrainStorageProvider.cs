// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Kernel.Grains.Observation;
using Aksio.Cratis.Observation;
using Aksio.DependencyInversion;
using Orleans.Runtime;
using Orleans.Storage;

namespace Aksio.Cratis.Kernel.Persistence.Observation;

/// <summary>
/// Represents an implementation of <see cref="IGrainStorage"/> for handling observer state storage.
/// </summary>
public class ObserverGrainStorageProvider : IGrainStorage
{
    readonly IExecutionContextManager _executionContextManager;
    readonly ProviderFor<IObserverStorage> _observerStorageProvider;
    readonly ProviderFor<IEventSequenceStorage> _eventSequenceStorageProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverGrainStorageProvider"/> class.
    /// </summary>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with the execution context.</param>
    /// <param name="observerStorageProvider">Provider for <see cref="IObserverStorage"/>.</param>
    /// <param name="eventSequenceStorageProvider">Provider for <see cref="IEventSequenceStorage"/>.</param>
    public ObserverGrainStorageProvider(
        IExecutionContextManager executionContextManager,
        ProviderFor<IObserverStorage> observerStorageProvider,
        ProviderFor<IEventSequenceStorage> eventSequenceStorageProvider)
    {
        _executionContextManager = executionContextManager;
        _observerStorageProvider = observerStorageProvider;
        _eventSequenceStorageProvider = eventSequenceStorageProvider;
    }

    /// <inheritdoc/>
    public Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var actualGrainState = (grainState as IGrainState<ObserverState>)!;
        var observerId = (ObserverId)grainId.GetGuidKey(out var observerKeyAsString);
        var observerKey = ObserverKey.Parse(observerKeyAsString!);
        var eventSequenceId = observerKey.EventSequenceId;

        _executionContextManager.Establish(observerKey.TenantId, CorrelationId.New(), observerKey.MicroserviceId);
        actualGrainState.State = await _observerStorageProvider().GetState(observerId, observerKey);

        if (actualGrainState.State.LastHandledEventSequenceNumber == EventSequenceNumber.Unavailable)
        {
            var tail = await _eventSequenceStorageProvider().GetTailSequenceNumber(
                    actualGrainState.State.EventSequenceId,
                    actualGrainState.State.EventTypes);

            if (tail < actualGrainState.State.NextEventSequenceNumber)
            {
                actualGrainState.State = actualGrainState.State with
                {
                    LastHandledEventSequenceNumber = tail
                };
            }
        }

        if (actualGrainState.State.Handled == EventCount.NotSet)
        {
            var count = await _eventSequenceStorageProvider().GetCount(
                actualGrainState.State.EventSequenceId,
                actualGrainState.State.LastHandledEventSequenceNumber,
                actualGrainState.State.EventTypes);

            actualGrainState.State = actualGrainState.State with
            {
                Handled = count
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var actualGrainState = (grainState as IGrainState<ObserverState>)!;
        var observerId = grainId.GetGuidKey(out var observerKeyAsString);
        var observerKey = ObserverKey.Parse(observerKeyAsString!);
        var eventSequenceId = observerKey.EventSequenceId;
        _executionContextManager.Establish(observerKey.TenantId, CorrelationId.New(), observerKey.MicroserviceId);

        await _observerStorageProvider().SaveState(observerId, observerKey, actualGrainState.State);
    }
}
