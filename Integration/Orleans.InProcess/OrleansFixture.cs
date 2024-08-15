// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Concepts.EventSequences;
using Cratis.Chronicle.Concepts.Observation;
using Cratis.Chronicle.Grains;
using Cratis.Chronicle.Grains.EventSequences;
using Cratis.Chronicle.Grains.Observation;
using Cratis.Chronicle.Integration.Base;
using Cratis.Chronicle.Reactions;
using Cratis.Chronicle.Setup;
using Cratis.Chronicle.Storage;
using Cratis.Chronicle.Storage.EventSequences;
using Cratis.Json;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Orleans.Storage;

namespace Cratis.Chronicle.Integration.Orleans.InProcess;

public class OrleansFixture(GlobalFixture globalFixture) : WebApplicationFactory<Startup>, IClientArtifactsProvider
{
    string _name = string.Empty;

    protected override IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder();
        builder.UseCratisMongoDB(
            mongo =>
            {
                mongo.Server = "mongodb://localhost:27018";
                mongo.Database = "orleans";
            });

        builder.UseDefaultServiceProvider(_ => _.ValidateOnBuild = false);

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(Globals.JsonSerializerOptions);
            services.AddControllers();
            services.Configure<ChronicleOptions>(opts => opts.ArtifactsProvider = this);
            ConfigureServices(services);
        });

        builder.UseCratisChronicle();

        builder.UseOrleans(silo =>
            {
                silo
                    .UseLocalhostClustering()
                    .AddCratisChronicle(
                        options => options.EventStoreName = "sample",
                        _ => _.WithMongoDB());
            })
            .UseConsoleLifetime();

        // For some weird reason we need this https://stackoverflow.com/questions/69974249/no-app-configured-error-while-using-webapplicationfactory-for-running-integrat
        builder.ConfigureWebHostDefaults(b => b.Configure(app => { }));

        return builder;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSolutionRelativeContentRoot("Integration/Orleans.InProcess");
    }

    public INetwork Network => GlobalFixture.Network;
    public MongoDBDatabase EventStoreDatabase => GlobalFixture.EventStore;
    public MongoDBDatabase EventStoreForNamespaceDatabase => GlobalFixture.EventStoreForNamespace;
    public MongoDBDatabase ReadModelsDatbase => GlobalFixture.ReadModels;

    public IEventStore EventStore => Services.GetRequiredService<IEventStore>();
    public IChronicleClient ChronicleClient => Services.GetRequiredService<IChronicleClient>();
    public IEventStoreStorage EventStoreStorage => Services.GetRequiredService<IStorage>().GetEventStore("sample");
    public IEventStoreNamespaceStorage GetEventStoreNamespaceStorage(Concepts.EventStoreNamespaceName? namespaceName = null) => EventStoreStorage.GetNamespace(namespaceName ?? Concepts.EventStoreNamespaceName.Default);
    public IEventSequenceStorage GetEventLogStorage(Concepts.EventStoreNamespaceName? namespaceName = null) => GetEventStoreNamespaceStorage(namespaceName).GetEventSequence(EventSequenceId.Log);

    public TStorage GetGrainStorage<TStorage>(string key)
        where TStorage : IGrainStorage => (TStorage)Services.GetRequiredKeyedService<IGrainStorage>(key);
    public EventSequencesStorageProvider GetEventSequenceStatesStorage() => GetGrainStorage<EventSequencesStorageProvider>(WellKnownGrainStorageProviders.EventSequences);
    public IEventSequence EventLogSequenceGrain => GetEventSequenceGrain(EventSequenceId.Log);
    public IEventSequence GetEventSequenceGrain(EventSequenceId id) => Services.GetRequiredService<IGrainFactory>().GetGrain<IEventSequence>(CreateEventSequenceKey(id));
    public EventSequenceKey CreateEventSequenceKey(EventSequenceId id) => new(id, "sample", Concepts.EventStoreNamespaceName.Default);

    public IObserver GetObserverFor<T>() => Services.GetRequiredService<IGrainFactory>()
        .GetGrain<IObserver>(
            new ObserverKey(typeof(T).GetReactionId().Value,
            "sample",
            Concepts.EventStoreNamespaceName.Default,
            EventSequenceId.Log));
    public GlobalFixture GlobalFixture { get; } = globalFixture;

    public void SetName(string name) => _name = name;

    protected void EnsureBuilt()
    {
        Services.GetRequiredService<IEventStore>();
    }

    protected override void Dispose(bool disposing)
    {
        GlobalFixture.PerformBackup(_name);
        GlobalFixture.RemoveAllDatabases().GetAwaiter().GetResult();
        base.Dispose(disposing);
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    public virtual IEnumerable<Type> EventTypes { get; } = [];
    public virtual IEnumerable<Type> Projections { get; } = [];
    public virtual IEnumerable<Type> Adapters { get; } = [];
    public virtual IEnumerable<Type> Reactions { get; } = [];
    public virtual IEnumerable<Type> Reducers { get; } = [];
    public virtual IEnumerable<Type> ReactionMiddlewares { get; } = [];
    public virtual IEnumerable<Type> ComplianceForTypesProviders { get; } = [];
    public virtual IEnumerable<Type> ComplianceForPropertiesProviders { get; } = [];
    public virtual IEnumerable<Type> Rules { get; } = [];
    public virtual IEnumerable<Type> AdditionalEventInformationProviders { get; } = [];
    public virtual IEnumerable<Type> AggregateRoots { get; } = [];
    public virtual IEnumerable<Type> AggregateRootStateTypes { get; } = [];
    public virtual IEnumerable<Type> ConstraintTypes { get; } = [];
    public virtual IEnumerable<Type> UniqueConstraints { get; } = [];
    public virtual IEnumerable<Type> UniqueEventTypeConstraints { get; } = [];
    public void Initialize()
    {
    }
}