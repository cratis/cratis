// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Orleans;

[EventType]
public record QuantityAdjustedForItemInCart(PersonId PersonId, MaterialId MaterialId, int Quantity);
