// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace Cratis.Kernel.Domain.EventSequences;

public record Event(uint Sequence, string Name, DateTimeOffset Occurred, JsonDocument Content);
