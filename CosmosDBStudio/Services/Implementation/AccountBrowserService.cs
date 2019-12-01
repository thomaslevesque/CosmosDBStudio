using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class AccountBrowserService : IAccountBrowserService
    {
        private readonly IAccountDirectory _accountDirectory;

        public AccountBrowserService(IAccountDirectory _accountDirectory)
        {
            this._accountDirectory = _accountDirectory;
        }

        public async Task<string[]> GetDatabasesAsync(string accountId)
        {
            var account = _accountDirectory.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            using var client = CreateCosmosClient(account);
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
            var account = _accountDirectory.GetById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            using var client = CreateCosmosClient(account);
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

        private CosmosClient CreateCosmosClient(CosmosAccount account)
        {
            // TODO: connection options
            return new CosmosClient(
                account.Endpoint,
                account.Key);
        }
    }
}