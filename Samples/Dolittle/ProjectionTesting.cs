// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using Cratis.Boot;
using Cratis.Dynamic;
using Cratis.Events.Projections;

namespace Sample
{
    public class ProjectionTesting : IPerformBootProcedure
    {
        const string EventTypeA = "d3e83b35-c11e-46cc-91ca-58cd66d1aa9e";
        const string EventTypeB = "5addd1d0-c270-4d86-87b5-13b7b97b1dde";
        const string EventTypeC = "fe3c32a7-a50d-47ba-bfeb-cf2417453de8";

        class TestProvider : IProjectionEventProvider
        {
            public ProjectionEventProviderTypeId TypeId => "edc4e112-0a6f-4c68-86ca-646d789d0541";

            public void ProvideFor(IProjectionPipeline pipeline, ISubject<Event> subject)
            {
                subject.OnNext(new Event(0, EventTypeA, DateTimeOffset.UtcNow, "d567f175-f940-4f4d-88ee-d96885a78c1a", new { integer = 42, a_string = "Forty Two" }.AsExpandoObject()));
                subject.OnNext(new Event(0, EventTypeB, DateTimeOffset.UtcNow, "d567f175-f940-4f4d-88ee-d96885a78c1a", new { moreStuff = 43 }.AsExpandoObject()));
            }

            public Task<IEventCursor> GetFromPosition(IProjection projection, EventLogSequenceNumber start) => throw new NotImplementedException();

        }

        public void Perform()
        {
            // var parser = new JsonProjectionParser();
            // var projection = parser.Parse(File.ReadAllText("./projection.json"));
            // var provider = new TestProvider();
            // var pipeline = new ProjectionPipeline(provider, projection);
            // var storage = new InMemoryProjectionStorage();
            // pipeline.StoreIn(storage);
            //pipeline.Start();
        }
    }
}
