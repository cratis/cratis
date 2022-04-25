// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

public class PivotFacetCategory
{
    public string IsFilterVisible { get; set; } = BooleanAsString.FALSE;
    public string IsMetaDataVisible { get; set; } = BooleanAsString.FALSE;
    public string IsWordWheelVisible { get; set; } = BooleanAsString.FALSE;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
