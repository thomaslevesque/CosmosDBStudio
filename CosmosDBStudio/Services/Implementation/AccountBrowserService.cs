using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class AccountBrowserService : IAccountBrowserService
    {
        private readonly IAccountDirectory _accountDirectory;
        private readonly IClientPool _clientPool;

        public AccountBrowserService(IAccountDirectory accountDirectory, IClientPool clientPool)
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
    }
}