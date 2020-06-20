using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Task CreateDatabaseAsync(string accountId, CosmosDatabase database)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            return client.CreateDatabaseAsync(database.Id, database.Throughput);
        }

        public Task<int?> GetDatabaseThroughputAsync(string accountId, string databaseId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            return database.ReadThroughputAsync();
        }

        public Task SetDatabaseThroughputAsync(string accountId, string databaseId, int throughput)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            return database.ReplaceThroughputAsync(throughput);
        }

        public Task DeleteDatabaseAsync(string accountId, string databaseId)
        {
            var client = _clientPool.GetClientForAccount(accountId);
            var database = client.GetDatabase(databaseId);
            return database.DeleteAsync();
        }
    }
}