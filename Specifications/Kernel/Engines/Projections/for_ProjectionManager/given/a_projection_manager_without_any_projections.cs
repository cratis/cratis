// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Engines.Projections.Pipelines;

namespace Aksio.Cratis.Kernel.Engines.Projections.for_ProjectionManager.given;

public class a_projection_manager_without_any_projections : Specification
{
    protected ProjectionManager manager;
    protected Mock<IProjectionFactory> projection_factory;
    protected Mock<IProjectionPipelineFactory> projection_pipeline_factory;

    void Establish()
    {
        projection_factory = new();
        projection_pipeline_factory = new();
        manager = new ProjectionManager(
            projection_factory.Object,
            projection_pipeline_factory.Object);
    }
}