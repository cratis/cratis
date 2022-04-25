/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { QueryFor, QueryResult, useQuery, PerformQuery } from '@aksio/cratis-applications-frontend/queries';
import { JsonResult } from './JsonResult';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/events/store/pivotviewer');

export class All extends QueryFor<JsonResult> {
    readonly route: string = '/api/events/store/pivotviewer';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: JsonResult = {} as any;

    get requestArguments(): string[] {
        return [
        ];
    }

    static use(): [QueryResult<JsonResult>, PerformQuery] {
        return useQuery<JsonResult, All>(All);
    }
}
