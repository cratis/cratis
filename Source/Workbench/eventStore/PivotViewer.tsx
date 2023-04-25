// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useEffect, useRef } from 'react';
import { useExternalScript } from './useExternalScript';




export const PivotViewer = () => {
    const pivotViewerContainer = useRef<HTMLDivElement>(null);
    const $ = (window as any).$;

    const loadingState = useExternalScript('/scripts/pivotviewer.min.js');

    useEffect(() => {
        if (loadingState !== 'ready') {
            return;
        }
        const GlobalPivotViewer = (window as any).PivotViewer;

        ($(pivotViewerContainer.current!) as any).PivotViewer({
            Loader: new GlobalPivotViewer.Models.Loaders.JSONLoader('api/events/store/pivotviewer'),
            //Loader: new PivotViewer.Models.Loaders.JSONLoader('samples/data/t.json'),
            //Loader: new PivotViewer.Models.Loaders.CXMLLoader("samples/data/simple_ski.cxml"),
            ImageController: new GlobalPivotViewer.Views.SimpleImageController()
        });
    }, [loadingState]);

    return (
        <div style={{ height: '100%' }} ref={pivotViewerContainer}>
        </div>
    );
};
