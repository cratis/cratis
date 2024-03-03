// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Json.for_TypeWithObjectPropertiesJsonConverter;

public record SuperType(string StringValue, int IntegerProperty, object FirstObjectProperty, object SecondObjectProperty, string AdditionalString) : TypeWithObjectProperties(StringValue, IntegerProperty, FirstObjectProperty, SecondObjectProperty);
