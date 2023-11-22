// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Aggregates.for_AggregateRootEventHandlersFactory;

public class when_creating_for_stateful_aggregate_root : Specification
{
    AggregateRootEventHandlersFactory factory;

    IAggregateRootEventHandlers result;

    void Establish() => factory = new();

    void Because() => result = factory.CreateFor(new StatefulAggregateRoot());

    [Fact] void should_return_a_null_implementation() => result.ShouldBeOfExactType<NullAggregateRootEventHandlers>();
}