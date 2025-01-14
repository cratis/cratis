// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Events;

namespace Cratis.Chronicle.Orleans.Aggregates.for_AggregateRootFactory.when_getting;

public class and_aggregate_root_type_is_correct : given.an_aggregate_root_factory
{
    CorrectAggregateRoot _result;
    CorrectAggregateRoot _expectedAggregateRoot;

    void Establish()
    {
        _expectedAggregateRoot = new CorrectAggregateRoot();
        _grainFactory.GetGrain(typeof(CorrectAggregateRoot), Arg.Any<string>()).Returns(_expectedAggregateRoot);
    }

    async Task Because() => _result = await _factory.Get<CorrectAggregateRoot>(EventSourceId.New());

    [Fact] void should_return_the_instance() => _result.ShouldEqual(_expectedAggregateRoot);
}

