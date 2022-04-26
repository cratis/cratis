/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { QueryFor, QueryResult, useQuery, PerformQuery } from '@aksio/cratis-applications-frontend/queries';
import { JsonResult } from './JsonResult';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/events/store/pivotviewer//api/events/store/images/imagelist.json');

export class ImageList extends QueryFor<JsonResult> {
    readonly route: string = '/api/events/store/pivotviewer//api/events/store/images/imagelist.json';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: JsonResult = {} as any;

    get requestArguments(): string[] {
        return [
        ];
    }

    static use(): [QueryResult<JsonResult>, PerformQuery] {
        return useQuery<JsonResult, ImageList>(ImageList);
    }
}
