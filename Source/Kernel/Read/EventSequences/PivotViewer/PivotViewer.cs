// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Read.EventSequences.PivotViewer;

public class PivotViewer
{
    public string CollectionName { get; set; } = string.Empty;
    public PivotFacetCategories FacetCategories { get; set; } = new();
    public PivotItems Items { get; set; } = new();
    public PivotViewerExtensions Extensions { get; set; } = new();
}