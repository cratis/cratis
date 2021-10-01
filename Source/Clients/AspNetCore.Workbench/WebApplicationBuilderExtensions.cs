// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extensions for using Cratis with a <see cref="WebApplicationBuilder"/>.
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Use Cratis workbench.
        /// </summary>
        /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder"/> to build on.</param>
        /// <returns><see cref="WebApplicationBuilder"/> for configuration continuation.</returns>
        public static WebApplicationBuilder UseCratisWorkbench(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.UseCratisWorkbench();
            return webApplicationBuilder;
        }
    }
}
