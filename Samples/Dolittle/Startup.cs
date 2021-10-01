// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Types;

namespace Sample
{
    public class Startup
    {
        internal static readonly ITypes Types = new Types();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSingleton(Types);
            services.AddDolittleSchemaStore("localhost", 27017);
            services.AddCratisWorkbench();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.AddDolittleSchemaStore();
            app.AddCratisWorkbench();
        }
    }
}
