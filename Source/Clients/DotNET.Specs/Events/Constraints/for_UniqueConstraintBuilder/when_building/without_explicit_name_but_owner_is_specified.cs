// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Events.Constraints.for_UniqueConstraintBuilder.when_adding_on_using_event_type_directly;
using Microsoft.VisualBasic;

namespace Cratis.Chronicle.Events.Constraints.for_UniqueConstraintBuilder.when_building;

public class without_explicit_name_but_owner_is_specified : given.a_unique_constraint_builder_with_owner
{
    IConstraintDefinition _result;

    void Establish()
    {
        var eventType = new EventType(nameof(EventWithStringProperty), EventGeneration.First);
        _constraintBuilder.On(eventType, nameof(EventWithStringProperty.SomeProperty));
    }

    void Because() => _result = _constraintBuilder.Build();

    [Fact] void should_set_name_to_owners_name() => _result.Name.Value.ShouldEqual(nameof(Owner));
}
