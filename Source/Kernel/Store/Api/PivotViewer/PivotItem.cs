// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

public class PivotItem
{
    public PivotItemFacets Facets { get; set; } = new();
    public string Id { get; set; } = string.Empty;
    public string Extension { get; set; } = "\n\t";
    public string Img { get; set; } = string.Empty;
    public string Href { get; set; } = "#";
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
