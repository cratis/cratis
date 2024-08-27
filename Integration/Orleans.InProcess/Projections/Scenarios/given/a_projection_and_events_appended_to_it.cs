// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Events;
using Cratis.Chronicle.EventSequences;
using Cratis.Chronicle.Integration.Base;
using Cratis.Chronicle.Integration.Orleans.InProcess.Projections.ProjectionTypes;
using MongoDB.Driver;

namespace Cratis.Chronicle.Integration.Orleans.InProcess.Projections.Scenarios.given;

public class a_projection_and_events_appended_to_it<TProjection, TModel>(GlobalFixture globalFixture) : IntegrationSpecificationContext(globalFixture)
    where TProjection : class, IProjectionFor<TModel>, new()
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    GlobalFixture _globalFixture = globalFixture;
#pragma warning restore CA2213 // Disposable fields should be disposed

    public EventSourceId EventSourceId;
    public string ModelId;

    public TModel Result;
    public AppendedEvent[] AppendedEvents;
    public override IEnumerable<Type> Projections => [typeof(TProjection)];
    protected List<object> EventsToAppend = [];
    protected List<EventAndEventSourceId> EventsWithEventSourceIdToAppend = [];

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(new TProjection());
    }

    void Establish()
    {
        EventSourceId = Guid.NewGuid().ToString();
        ModelId = EventSourceId;
    }

    async Task Because()
    {
        var observer = GetObserverForProjection<TProjection>();
        await observer.WaitTillActive();

        AppendResult appendResult = null;
        foreach (var @event in EventsToAppend)
        {
            appendResult = await EventStore.EventLog.Append(EventSourceId, @event);
        }

        foreach (var @event in EventsWithEventSourceIdToAppend)
        {
            appendResult = await EventStore.EventLog.Append(@event.EventSourceId, @event.Event);
        }

        var appendedEvents = await EventStore.EventLog.GetForEventSourceIdAndEventTypes(EventSourceId, EventTypes.Select(_ => _.GetEventType()));
        AppendedEvents = [.. appendedEvents];

        if (appendResult is not null)
        {
            await observer.WaitTillReachesEventSequenceNumber(appendResult.SequenceNumber);
            var filter = Builders<TModel>.Filter.Eq(new StringFieldDefinition<TModel, string>("_id"), ModelId);
            var result = await _globalFixture.ReadModels.Database.GetCollection<TModel>().FindAsync(filter);
            Result = result.FirstOrDefault();
        }
    }
}