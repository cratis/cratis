// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Aksio.Cratis.Applications;
using Aksio.Cratis.DependencyInversion;
using Aksio.Cratis.Dynamic;
using Aksio.Cratis.Events.Schemas;
using Aksio.Cratis.Execution;
using Microsoft.AspNetCore.Mvc;

namespace Aksio.Cratis.Events.Store.Api.PivotViewer;

#pragma warning disable SA1600

[Route("/api/events/store/pivotviewer")]
[SkipEnvelope]
public class PivotViewerController : Controller
{
    readonly ProviderFor<IEventSequenceStorageProvider> _eventSequenceStorageProviderProvider;
    readonly ProviderFor<ISchemaStore> _schemaStore;
    readonly IExecutionContextManager _executionContextManager;

    public PivotViewerController(
        ProviderFor<IEventSequenceStorageProvider> eventSequenceStorageProviderProvider,
        ProviderFor<ISchemaStore> schemaStore,
        IExecutionContextManager executionContextManager)
    {
        _eventSequenceStorageProviderProvider = eventSequenceStorageProviderProvider;
        _schemaStore = schemaStore;
        _executionContextManager = executionContextManager;
    }

    [HttpGet]
    public async Task<JsonResult> All()
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
                    new() { Name = "Event Source", Type = "String", IsFilterVisible = BooleanAsString.TRUE },
                    new() { Name = "Occurred", Type = "DateTime", IsFilterVisible = BooleanAsString.TRUE },
                    new() { Name = "Content", Type = "String", IsFilterVisible = BooleanAsString.TRUE, IsMetaDataVisible = BooleanAsString.TRUE, IsWordWheelVisible = BooleanAsString.TRUE }
                }
            }
        };
        pivotViewer.Items.ImgBase = "/images";

        _executionContextManager.Establish(TenantId.Development, CorrelationId.New(), MicroserviceId.Unspecified);

        var events = new List<AppendedEvent>();
        var cursor = await _eventSequenceStorageProviderProvider().GetFromSequenceNumber(EventSequenceNumber.First);
        while (await cursor.MoveNext())
        {
            events.AddRange(cursor.Current);
        }

        pivotViewer.Items.Item = events.Select(_ =>
        {
            var task = _schemaStore().GetFor(_.Metadata.Type.Id);
            task.Wait();
            var type = task.Result;
            var name = type.Schema.GetDisplayName();
            return new PivotItem
            {
                Id = _.Metadata.SequenceNumber.ToString(),
                Img = "event.png",
                Name = name,
                Facets = new()
                {
                    Facet = new PivotItemFacet[]
                    {
                        new StringPivotItemFacet("Event", name),
                        new StringPivotItemFacet("Event Source", _.Context.EventSourceId.Value),
                        new DateTimePivotItemFacet("Occurred", _.Context.Occurred),
                        new StringArrayPivotItemFacet("Content", _.Content.Select(kvp => $"{kvp.Key} : {kvp.Value}"))
                    }
                }
            };
        });

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IgnoreReadOnlyProperties = false,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        return new JsonResult(pivotViewer.AsExpandoObject(), options);
    }

    [HttpGet("/api/events/store/images/imagelist.json")]
    public Task<JsonResult> ImageList()
    {
        var imageList = new
        {
            ImageFiles = new string[]
            {
                "event.png"
            }
        };

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = null
        };
        return Task.FromResult(new JsonResult(imageList, options));
    }
}
