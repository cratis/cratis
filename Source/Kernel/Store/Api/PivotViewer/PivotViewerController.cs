// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Aksio.Cratis.Applications;
using Microsoft.AspNetCore.Mvc;

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

[Route("/api/events/store/pivotviewer")]
[SkipEnvelope]
public class PivotViewerController : Controller
{
    [HttpGet]
    public Task<JsonResult> All()
    {
        var pivotViewer = new PivotViewer
        {
            CollectionName = "Events",
            Extensions = new()
            {
                Copyright = new("https://aksio.no", "Aksio Cratis")
            },
            FacetCategories = new()
            {
                FacetCategory = new PivotFacetCategory[]
                {
                    new() { Name = "Event", Type = "String", IsFilterVisible = BooleanAsString.TRUE },
                    new() { Name = "Occurred", Type = "DateTime", IsFilterVisible = BooleanAsString.TRUE },
                    new() { Name = "Content", Type = "String", IsFilterVisible = BooleanAsString.TRUE, IsMetaDataVisible = BooleanAsString.TRUE, IsWordWheelVisible = BooleanAsString.TRUE }
                }
            }
        };
        pivotViewer.Items.ImgBase = "images";
        pivotViewer.Items.Item = new PivotItem[]
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Img = "event.png",
                Name = "Something",
                Facets = new()
                {
                    Facet = new PivotItemFacet[]
                    {
                        new StringPivotItemFacet("Event", "Something"),
                        new DateTimePivotItemFacet("Occurred", DateTimeOffset.UtcNow)
                    }
                }
            }
        };

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = null
        };
        return Task.FromResult(new JsonResult(pivotViewer, options));
    }
}
