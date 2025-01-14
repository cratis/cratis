// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Aggregates;
using Cratis.Chronicle.Events;
using Cratis.Chronicle.Integration.Base;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Domain;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Events;
using Cratis.Chronicle.Orleans.Aggregates;
using Cratis.Chronicle.Transactions;

namespace Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Scenarios.given;

public class context_for_aggregates(GlobalFixture globalFixture) : IntegrationSpecificationContext(globalFixture)
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    protected GlobalFixture _globalFixture = globalFixture;
    public IUnitOfWork UnitOfWork;
#pragma warning restore CA2213 // Disposable fields should be disposed
    public bool IsNew;

    public override IEnumerable<Type> AggregateRoots => [typeof(User)];
    public override IEnumerable<Type> EventTypes => [typeof(UserOnBoarded), typeof(UserCreated), typeof(UserDeleted), typeof(UserNameChanged)];

    protected List<EventAndEventSourceId> EventsWithEventSourceIdToAppend = [];
    protected IAggregateRootFactory AggregateRootFactory => Services.GetRequiredService<IAggregateRootFactory>();
    protected IUnitOfWorkManager UnitOfWorkManager => Services.GetRequiredService<IUnitOfWorkManager>();
    protected ICorrelationIdAccessor CorrelationIdAccessor => Services.GetRequiredService<ICorrelationIdAccessor>();
    protected CorrelationId CorrelationId;

    protected override void ConfigureServices(IServiceCollection services)
    {
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
}
