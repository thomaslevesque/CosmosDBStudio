using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var container = client.GetContainer(databaseId, containerId);
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
            var c = client.GetContainer(databaseId, container.Id);
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
            var container = client.GetContainer(databaseId, containerId);
            return container.DeleteContainerAsync();
        }

        public async Task<CosmosStoredProcedure[]> GetStoredProceduresAsync(string accountId, string databaseId, string containerId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var container = client.GetContainer(databaseId, containerId);
            var iterator = container.Scripts.GetStoredProcedureQueryIterator<StoredProcedureProperties>();
            var storedProcedures = new List<CosmosStoredProcedure>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                storedProcedures.AddRange(response.Select(sp => new CosmosStoredProcedure
                {
                    Id = sp.Id,
                    Body = sp.Body,
                    ETag = sp.ETag
                }));
            }

            return storedProcedures.ToArray();
        }

        public async Task<CosmosUserDefinedFunction[]> GetUserDefinedFunctionsAsync(string accountId, string databaseId, string containerId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var container = client.GetContainer(databaseId, containerId);
            var iterator = container.Scripts.GetUserDefinedFunctionQueryIterator<UserDefinedFunctionProperties>();
            var functions = new List<CosmosUserDefinedFunction>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                functions.AddRange(response.Select(udf => new CosmosUserDefinedFunction
                {
                    Id = udf.Id,
                    Body = udf.Body,
                    ETag = udf.ETag,
                }));
            }

            return functions.ToArray();
        }

        public async Task<CosmosTrigger[]> GetTriggersAsync(string accountId, string databaseId, string containerId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var container = client.GetContainer(databaseId, containerId);
            var iterator = container.Scripts.GetTriggerQueryIterator<TriggerProperties>();
            var triggers = new List<CosmosTrigger>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                triggers.AddRange(response.Select(t => new CosmosTrigger
                {
                    Id = t.Id,
                    Body = t.Body,
                    ETag = t.ETag,
                    Operation = t.TriggerOperation,
                    Type = t.TriggerType
                }));
            }

            return triggers.ToArray();
        }
    }
}