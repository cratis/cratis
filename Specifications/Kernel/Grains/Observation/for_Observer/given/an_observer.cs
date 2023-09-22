// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Kernel.Observation;
using Aksio.Json;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;

namespace Aksio.Cratis.Kernel.Grains.Observation.for_Observer.given;

public class an_observer : GrainSpecification<ObserverState>
{
    protected Observer observer;
    protected Mock<IEventSequenceStorage> event_sequence_storage;
    protected Mock<IStreamProvider> stream_provider;
    protected Mock<IStreamProvider> sequence_stream_provider;
    protected Mock<IObserverSubscriber> subscriber;
    protected Mock<IPersistentState<FailedPartitions>> failed_partitions_persistent_state;
    protected ObserverKey ObserverKey => new(MicroserviceId.Unspecified, TenantId.NotSet, EventSequenceId.Log);
    protected List<FailedPartitions> written_failed_partitions_states = new();
    protected FailedPartitions failed_partitions_state;

    protected override Guid GrainId => Guid.Parse("d2a138a2-6ca5-4bff-8a2f-ffd8534cc80e");

    protected override string GrainKeyExtension => ObserverKey;

    protected override Grain GetGrainInstance()
    {
        event_sequence_storage = new();
        failed_partitions_persistent_state = new();
        failed_partitions_state = new();
        failed_partitions_persistent_state.SetupGet(_ => _.State).Returns(failed_partitions_state);

        failed_partitions_persistent_state.Setup(_ => _.WriteStateAsync()).Callback(() =>
            {
                var serialized = JsonSerializer.Serialize(failed_partitions_state, Globals.JsonSerializerOptions);
                var clone = JsonSerializer.Deserialize<FailedPartitions>(serialized, Globals.JsonSerializerOptions);
                written_failed_partitions_states.Add(clone);
            }).Returns(Task.CompletedTask);

        observer = new Observer(
            () => event_sequence_storage.Object,
            failed_partitions_persistent_state.Object,
            Mock.Of<ILogger<Observer>>());

        return observer;
    }

    protected override void OnBeforeGrainActivate()
    {
        sequence_stream_provider = new();
        stream_provider_collection.Setup(_ => _.GetService(service_provider.Object, WellKnownProviders.EventSequenceStreamProvider)).Returns(sequence_stream_provider.Object);
        subscriber = new();
        grain_factory.Setup(_ => _.GetGrain(typeof(ObserverSubscriber), GrainId, IsAny<string>())).Returns(subscriber.Object);
    }

    protected override void OnAfterGrainActivate()
    {
        written_states.Clear();
    }
}