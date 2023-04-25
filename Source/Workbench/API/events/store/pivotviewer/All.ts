/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { QueryFor, QueryResultWithState, useQuery, PerformQuery } from '@aksio/cratis-applications-frontend/queries';
import { JsonResult } from './JsonResult';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/events/store/pivotviewer');

export class All extends QueryFor<JsonResult> {
    readonly route: string = '/api/events/store/pivotviewer';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: JsonResult = {} as any;

    constructor() {
        super(JsonResult, false);
    }

    get requestArguments(): string[] {
        return [
        ];
    }

    static use(): [QueryResultWithState<JsonResult>, PerformQuery] {
        return useQuery<JsonResult, All>(All);
    }
}
