// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Events;
using Cratis.Chronicle.EventSequences;

namespace Cratis.Chronicle.Integration.Orleans.InProcess.Projections.Scenarios.Models;

public record Group(
    EventSourceId Id,
    string Name,
    IEnumerable<UserOnGroup> Users,
    EventSequenceNumber __eventSequenceNumber = default);
