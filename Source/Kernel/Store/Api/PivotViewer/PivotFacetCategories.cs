// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

public class PivotFacetCategories
{
    public IEnumerable<PivotFacetCategory> FacetCategory { get; set; } = Array.Empty<PivotFacetCategory>();
}
