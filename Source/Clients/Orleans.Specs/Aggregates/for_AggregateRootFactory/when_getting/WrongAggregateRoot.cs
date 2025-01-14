// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Aggregates;

namespace Cratis.Chronicle.Orleans.Aggregates.for_AggregateRootFactory.when_getting;

public class WrongAggregateRoot : Chronicle.Aggregates.IAggregateRoot
{
    /// <inheritdoc/>
    public Task Apply(object @event) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<AggregateRootCommitResult> Commit() => throw new NotImplementedException();
}

