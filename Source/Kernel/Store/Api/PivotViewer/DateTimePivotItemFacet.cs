// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

public class DateTimePivotItemFacet : PivotItemFacet
{
    public DateTimeValue DateTime { get; init; } = new DateTimeValue(DateTimeOffset.UtcNow);

    public DateTimePivotItemFacet(string name, DateTimeOffset value)
    {
        Name = name;
        DateTime = new DateTimeValue(value);
    }
}
