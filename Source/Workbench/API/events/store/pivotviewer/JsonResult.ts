/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { field } from '@aksio/cratis-fundamentals';

import { Object } from './Object';

export class JsonResult {

    @field(Object)
    value?: Object;

    @field(Object)
    serializerSettings?: Object;

    @field(String)
    contentType?: string;

    @field(Number)
    statusCode?: number;
}
