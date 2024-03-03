// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Strings;

namespace Cratis.Changes.for_ObjectComparer;

public class when_comparing_object_with_comparable_that_are_not_equal : given.an_object_comparer
{
    record TheType(MyComparable Comparable);

    TheType left;
    TheType right;
    bool result;
    IEnumerable<PropertyDifference> differences;

    void Establish()
    {
        left = new(new(1));
        right = new(new(1));
    }

    void Because() => result = comparer.Equals(left, right, out differences);

    [Fact] void should_not_be_equal() => result.ShouldBeFalse();
    [Fact] void should_only_have_one_property_difference() => differences.Count().ShouldEqual(1);
    [Fact] void should_have_concept_property_as_difference() => differences.First().PropertyPath.Path.ShouldEqual(nameof(TheType.Comparable).ToCamelCase());
}
