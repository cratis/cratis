// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.MongoDB;
using Cratis.Chronicle.Orleans.Aggregates;
using Cratis.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

builder.UseCratisMongoDB(
    mongo =>
    {
        mongo.Server = "mongodb://localhost:27017";
        mongo.Database = "orleans";
    });

builder.Host.UseDefaultServiceProvider(_ => _.ValidateOnBuild = false);

builder.Services.AddSingleton(Globals.JsonSerializerOptions);

builder.Host.UseOrleans(silo =>
    {
        silo
            .UseDashboard(options =>
            {
                options.Host = "*";
                options.Port = 8081;
                options.HostSelf = true;
            })
            .UseLocalhostClustering()
                .AddChronicle(_ => _
                    .WithMongoDB());
    })
    .UseConsoleLifetime();

var app = builder.Build();
var f = app.Services.GetRequiredService<IMongoDBClientFactory>();

app.MapGet(
    "/",
    async (IAggregateRootFactory aggregateRootFactory) =>
    {
        var aggregateRoot = await aggregateRootFactory.Get<IMyAggregateRoot>("6fbd1b71-923d-4fa7-bf44-777dcb091218");
        await aggregateRoot.DoStuff();
    });

await app.RunAsync();
