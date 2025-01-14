// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Orleans.Aggregates;
namespace Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Domain.Interfaces;

public interface IAnother : IAggregateRoot
{
    Task DoSomething();
}
