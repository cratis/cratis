// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;

namespace Cratis.Events.Projections.Pipelines.JobSteps.for_Catchup.given
{
    public class two_cursors_in_sequence_with_ten_events_each : a_catchup_step
    {
        protected IEnumerable<Event> first_cursor_events;
        protected IEnumerable<Event> second_cursor_events;

        void Establish()
        {
            first_cursor_events = Enumerable
                        .Range(0, 10)
                        .Select(_ => new Event(
                                            (uint)_,
                                            new EventType(Guid.NewGuid(), 1),
                                            DateTimeOffset.UtcNow,
                                            Guid.NewGuid().ToString(),
                                            new ExpandoObject())).ToArray();

            second_cursor_events = Enumerable
                        .Range(0, 10)
                        .Select(_ => new Event(
                                            (uint)_,
                                            new EventType(Guid.NewGuid(), 1),
                                            DateTimeOffset.UtcNow,
                                            Guid.NewGuid().ToString(),
                                            new ExpandoObject())).ToArray();

            var first_cursor = new Mock<IEventCursor>();
            first_cursor.SetupSequence(_ => _.MoveNext())
                        .ReturnsAsync(true)
                        .ReturnsAsync(false);
            first_cursor.SetupGet(_ => _.Current).Returns(first_cursor_events);
            var second_cursor = new Mock<IEventCursor>();
            second_cursor.Setup(_ => _.MoveNext()).Returns(Task.FromResult(false));
            second_cursor.SetupSequence(_ => _.MoveNext())
                        .ReturnsAsync(true)
                        .ReturnsAsync(false);
            second_cursor.SetupGet(_ => _.Current).Returns(second_cursor_events);

            var third_cursor = new Mock<IEventCursor>();
            third_cursor.Setup(_ => _.MoveNext()).Returns(Task.FromResult(false));

            provider
                .SetupSequence(_ => _.GetFromPosition(projection.Object, IsAny<EventLogSequenceNumber>()))
                .ReturnsAsync(first_cursor.Object)
                .ReturnsAsync(second_cursor.Object)
                .ReturnsAsync(third_cursor.Object);
        }
    }
}
