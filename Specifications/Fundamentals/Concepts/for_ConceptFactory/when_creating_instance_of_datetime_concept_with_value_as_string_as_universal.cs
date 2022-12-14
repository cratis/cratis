// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace Aksio.Cratis.Concepts.for_ConceptFactory;

public class when_creating_instance_of_datetime_concept_with_value_as_string_as_universal : Specification
{
    DateTimeConcept result;
    string now;

    void Establish() => now = "2022-12-14T08:45:46.4595800Z";

    void Because() => result = ConceptFactory.CreateConceptInstance(typeof(DateTimeConcept), now) as DateTimeConcept;

    [Fact] void should_be_the_value_of_the_datetime() => result.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ").ShouldEqual(now);
}
