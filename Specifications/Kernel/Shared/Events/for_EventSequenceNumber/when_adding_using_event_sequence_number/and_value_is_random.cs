// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Events.for_EventSequenceNumber.when_adding_using_event_sequence_number;

public class and_value_is_random : Specification
{
    static Random random = new();
    EventSequenceNumber result;
    EventSequenceNumber value;

    void Establish() => value = (ulong)random.Next(0, 100);

    void Because() => result = value + (EventSequenceNumber)42UL;

    [Fact] void should_add_value() => result.ShouldEqual((EventSequenceNumber)(value.Value + 42));
}
