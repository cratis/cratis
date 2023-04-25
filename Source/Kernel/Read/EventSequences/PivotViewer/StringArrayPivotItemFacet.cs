// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Read.EventSequences.PivotViewer;

public class StringArrayPivotItemFacet : PivotItemFacet
{
    public IEnumerable<StringValue> String { get; init; } = Array.Empty<StringValue>();

    public StringArrayPivotItemFacet(string name, IEnumerable<string> values)
    {
        Name = name;
        String = values.Select(_ => new StringValue(_));
    }
}
