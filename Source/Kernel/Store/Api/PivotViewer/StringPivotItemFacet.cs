// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

public class StringPivotItemFacet : PivotItemFacet
{
    public StringValue StringValue { get; init; } = new StringValue(string.Empty);

    public StringPivotItemFacet(string name, string value)
    {
        Name = name;
        StringValue = new StringValue(value);
    }
}
