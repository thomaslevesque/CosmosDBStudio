using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using CosmosDBStudio.Extensions;
using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class CosmosAccountManager : ICosmosAccountManager
    {
        private readonly IAccountDirectory _accountDirectory;
        private readonly IClientPool _clientPool;

        public CosmosAccountManager(IAccountDirectory accountDirectory, IClientPool clientPool)
        {
            _accountDirectory = accountDirectory;
            _clientPool = clientPool;
        }

        public async Task<string[]> GetDatabasesAsync(string accountId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var iterator = client.GetDatabaseQueryIterator<DatabaseProperties>();
            var databases = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                databases.AddRange(response.Select(d => d.Id));
            }

            return databases.ToArray();
        }

        public async Task<string[]> GetContainersAsync(string accountId, string databaseId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var iterator = database.GetContainerQueryIterator<ContainerProperties>();
            var containers = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                containers.AddRange(response.Select(d => d.Id));
            }

            return containers.ToArray();
        }

        public async Task<CosmosDatabase> GetDatabaseAsync(string accountId, string databaseId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var throughput = await database.ReadThroughputAsync();
            return new CosmosDatabase
            {
                Id = databaseId,
                Throughput = throughput
            };
        }

        public Task CreateDatabaseAsync(string accountId, CosmosDatabase database)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            return client.CreateDatabaseAsync(database.Id, database.Throughput);
        }

        public async Task UpdateDatabaseAsync(string accountId, CosmosDatabase database)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var db = client.GetDatabase(database.Id);
            var throughput = await db.ReadThroughputAsync();
            if (database.Throughput.HasValue != throughput.HasValue)
                throw new InvalidOperationException("Cannot change whether throughput is provisioned for an existing database.");

            if (database.Throughput.HasValue && database.Throughput != throughput)
            {
                await db.ReplaceThroughputAsync(database.Throughput.Value);
            }
        }

        public Task DeleteDatabaseAsync(string accountId, string databaseId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            return database.DeleteAsync();
        }

        public async Task<CosmosContainer> GetContainerAsync(string accountId, string databaseId, string containerId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var container = database.GetContainer(containerId);
            var properties = (await container.ReadContainerAsync()).Resource;
            var throughput = await container.ReadThroughputAsync();
            return new CosmosContainer
            {
                Id = containerId,
                PartitionKeyPath = properties.PartitionKeyPath,
                LargePartitionKey = properties.PartitionKeyDefinitionVersion > PartitionKeyDefinitionVersion.V1,
                DefaultTTL = properties.DefaultTimeToLive,
                Throughput = throughput
            };
        }

        public Task CreateContainerAsync(string accountId, string databaseId, CosmosContainer container)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var properties = new ContainerProperties(container.Id, container.PartitionKeyPath)
            {
                DefaultTimeToLive = container.DefaultTTL,
                PartitionKeyDefinitionVersion = container.LargePartitionKey
                    ? PartitionKeyDefinitionVersion.V2
                    : PartitionKeyDefinitionVersion.V1
            };

            return database.CreateContainerAsync(properties, container.Throughput);
        }

        public async Task UpdateContainerAsync(string accountId, string databaseId, CosmosContainer container)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var c = database.GetContainer(container.Id);
            var properties = (await c.ReadContainerAsync()).Resource;
            var throughput = await c.ReadThroughputAsync();

            if (container.Throughput.HasValue != throughput.HasValue)
                throw new InvalidOperationException("Cannot change whether throughput is provisioned for an existing container.");

            if (container.LargePartitionKey != (properties.PartitionKeyDefinitionVersion > PartitionKeyDefinitionVersion.V1))
                throw new InvalidOperationException("Cannot change partition key definition version for an existing container.");

            if (container.PartitionKeyPath != properties.PartitionKeyPath)
                throw new InvalidOperationException("Cannot change partition key path for an existing container.");

            if (container.DefaultTTL != properties.DefaultTimeToLive)
            {
                properties.DefaultTimeToLive = container.DefaultTTL;
                await c.ReplaceContainerAsync(properties);
            }

            if (container.Throughput.HasValue && container.Throughput != throughput)
            {
                await c.ReplaceThroughputAsync(container.Throughput.Value);
            }
        }

        public Task DeleteContainerAsync(string accountId, string databaseId, string containerId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            var container = database.GetContainer(containerId);
            return container.DeleteContainerAsync();
        }
    }
}