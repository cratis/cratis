// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Events;

namespace Cratis.Chronicle.Orleans.Aggregates.for_AggregateRootFactory.when_getting;

public class and_aggregate_root_type_is_wrong : given.an_aggregate_root_factory
{
    Exception _exception;

    async Task Because() => _exception = await Catch.Exception(() => _factory.Get<WrongAggregateRoot>(EventSourceId.New()));

    [Fact] void should_throw_wrong_type_for_aggregate_root() => _exception.ShouldBeOfExactType<WrongTypeForAggregateRoot>();
}
