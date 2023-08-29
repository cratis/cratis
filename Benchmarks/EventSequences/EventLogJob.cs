// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Kernel.MongoDB;
using Aksio.Execution;
using Microsoft.Extensions.DependencyInjection;
using IEventSequence = Aksio.Cratis.Kernel.Grains.EventSequences.IEventSequence;

namespace Benchmarks.EventSequences;

public abstract class EventLogJob : BenchmarkJob
{
    protected IEventSequence? EventSequence { get; private set; }

    [IterationSetup]
    public void CleanEventStore()
    {
        SetExecutionContext();

        Database?.DropCollection(CollectionNames.EventLog);
        Database?.DropCollection(CollectionNames.EventSequences);
    }

    protected override void Setup()
    {
        base.Setup();

        var grainFactory = GlobalVariables.ServiceProvider.GetRequiredService<IGrainFactory>();
        EventSequence = grainFactory.GetGrain<IEventSequence>(EventSequenceId.Log, keyExtension: new MicroserviceAndTenant(GlobalVariables.MicroserviceId, TenantId.Development));

        SetExecutionContext();
    }

    protected async Task Perform(Func<IEventSequence, Task> action)
    {
        SetExecutionContext();
        await action(EventSequence!);
    }
}