// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Reflection.for_DictionaryExtensions.when_checking_if_type_is_dictionary;

public class and_it_is_not : Specification
{
    bool result;

    void Because() => result = typeof(string).IsDictionary();

    [Fact] void should_not_be_considered_a_dictionary() => result.ShouldBeFalse();
}
