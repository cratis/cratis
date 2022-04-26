// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useEffect, useRef } from 'react';

export const EventStorePivotViewer = () => {
    const pivotViewerContainer = useRef<HTMLDivElement>(null);

    useEffect(() => {

        ($(pivotViewerContainer.current!) as any).PivotViewer({
            Loader: new PivotViewer.Models.Loaders.JSONLoader('api/events/store/pivotviewer'),
            //Loader: new PivotViewer.Models.Loaders.JSONLoader('samples/data/t.json'),
            //Loader: new PivotViewer.Models.Loaders.CXMLLoader("samples/data/simple_ski.cxml"),
            ImageController: new PivotViewer.Views.SimpleImageController()
        });
    }, []);

    return (
        <div style={{ height: '100%' }} ref={pivotViewerContainer}>
        </div>
    );
};
