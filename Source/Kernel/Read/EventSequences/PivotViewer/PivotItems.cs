// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Read.EventSequences.PivotViewer;

public class PivotItems
{
    public IEnumerable<PivotItem> Item { get; set; } = Array.Empty<PivotItem>();
    public string ImgBase { get; set; } = string.Empty;
}
