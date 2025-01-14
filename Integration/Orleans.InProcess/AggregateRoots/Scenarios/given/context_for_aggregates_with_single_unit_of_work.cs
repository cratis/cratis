// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Aggregates;
using Cratis.Chronicle.Integration.Base;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Domain;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Events;
using Cratis.Chronicle.Transactions;

namespace Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Scenarios.given;

public class context_for_aggregates_with_single_unit_of_work(GlobalFixture globalFixture) : IntegrationSpecificationContext(globalFixture)
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    public IUnitOfWork UnitOfWork;
#pragma warning restore CA2213 // Disposable fields should be disposed

    public override IEnumerable<Type> AggregateRoots => [typeof(User)];
    public override IEnumerable<Type> EventTypes => [
        typeof(UserOnBoarded),
        typeof(UserCreated),
        typeof(UserDeleted),
        typeof(UserNameChanged),
        typeof(SomethingHappened),
        typeof(UserDidSomething)];

    public IUnitOfWorkManager UnitOfWorkManager => Services.GetRequiredService<IUnitOfWorkManager>();

    protected List<EventAndEventSourceId> EventsWithEventSourceIdToAppend = [];
    protected IAggregateRootFactory AggregateRootFactory => Services.GetRequiredService<IAggregateRootFactory>();
    protected CorrelationId CorrelationId;

    protected override void ConfigureServices(IServiceCollection services)
    {
    }

    protected async Task PerformInUnitOfWork(Func<Task> action)
    {
        UnitOfWorkManager.SetCurrent(UnitOfWork);
        await action();
    }

    void Establish()
    {
        CorrelationId = CorrelationId.New();
        UnitOfWork = UnitOfWorkManager.Begin(CorrelationId);
    }

    async Task Because()
    {
        foreach (var @event in EventsWithEventSourceIdToAppend)
        {
            await EventStore.EventLog.Append(@event.EventSourceId, @event.Event);
        }
    }

    void Destroy()
    {
        UnitOfWork.Dispose();
    }
}
