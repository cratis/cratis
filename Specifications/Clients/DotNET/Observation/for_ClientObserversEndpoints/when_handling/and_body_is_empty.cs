// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace Cratis.Observation.for_ClientObserversEndpoints.when_handling;

public class and_body_is_empty : Cratis.given.an_http_context
{
    void Establish() => route_values.Add("observerId", Guid.NewGuid().ToString());

    Task Because() => ClientObserversEndpoints.Handler(http_context.Object);

    [Fact] void should_return_bad_request() => response.VerifySet(_ => _.StatusCode = (int)HttpStatusCode.BadRequest);
}
