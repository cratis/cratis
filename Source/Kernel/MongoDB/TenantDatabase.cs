// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Configuration;
using Aksio.MongoDB;
using MongoDB.Driver;

namespace Aksio.Cratis.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="ITenantDatabase"/>.
/// </summary>
[SingletonPerTenant]
public class TenantDatabase : ITenantDatabase
{
    readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="SharedDatabase"/> class.
    /// </summary>
    /// <param name="executionContext">Current <see cref="ExecutionContext"/>.</param>
    /// <param name="clientFactory"><see cref="IMongoDBClientFactory"/> for working with MongoDB.</param>
    /// <param name="configuration"><see cref="Storage"/> configuration.</param>
    public TenantDatabase(
        ExecutionContext executionContext,
        IMongoDBClientFactory clientFactory,
        Storage configuration)
    {
        var builder = new MongoUrlBuilder(configuration.Cluster.ConnectionDetails.ToString());
        var tenantIdAsString = executionContext.TenantId.ToString();
        var endOfFirstPartIndex = tenantIdAsString.IndexOf('-');
        builder.DatabaseName = $"cratis-{tenantIdAsString.Substring(0, endOfFirstPartIndex)}";
        var url = builder.ToMongoUrl();
        var client = clientFactory.Create(url);
        _database = client.GetDatabase(url.DatabaseName);
    }

    /// <inheritdoc/>
    public IMongoCollection<T> GetCollection<T>(string? collectionName = null)
    {
        if (collectionName == null)
        {
            return _database.GetCollection<T>();
        }

        return _database.GetCollection<T>(collectionName);
    }
}