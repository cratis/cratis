// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Concepts.Events;
using Cratis.Chronicle.Concepts.Jobs;
using Cratis.Chronicle.Concepts.Keys;
using Cratis.Chronicle.Grains.Observation.Jobs;
namespace Cratis.Chronicle.Grains.Observation.for_Observer;

public class when_replaying_partition_to_specific_event_sequnce_number : given.an_observer
{
    static Key partition;
    static EventSequenceNumber toSequenceNumber;

    void Establish()
    {
        partition = "some-partition";
        toSequenceNumber = 12;
    }

    Task Because() => observer.ReplayPartitionTo(partition, toSequenceNumber);

    [Fact] void should_add_partition_to_replaying_partitions() => state_storage.State.ReplayingPartitions.ShouldContain(partition);
    [Fact] void should_write_state_once() => storage_stats.Writes.ShouldEqual(1);
    [Fact] void should_start_replay_observer_partition_job() => jobsManager.Verify(_ => _.Start<IReplayObserverPartition, ReplayObserverPartitionRequest>(
        IsAny<JobId>(),
        Is<ReplayObserverPartitionRequest>(_ => _.FromSequenceNumber == EventSequenceNumber.First && _.ToSequenceNumber == toSequenceNumber)));
}