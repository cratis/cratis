// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.EventSequences;
using Cratis.Chronicle.Transactions;

namespace Cratis.Chronicle.Orleans.Aggregates.for_AggregateRootFactory.given;

public class an_aggregate_root_factory : Specification
{
    protected AggregateRootFactory _factory;
    protected IGrainFactory _grainFactory;

    void Establish()
    {
        _grainFactory = Substitute.For<IGrainFactory>();

        _factory = new AggregateRootFactory(_grainFactory);
    }
}
