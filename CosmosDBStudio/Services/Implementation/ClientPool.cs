using System;
using System.Collections.Concurrent;
using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class ClientPool : IClientPool
    {
        private readonly IAccountDirectory _accountDirectory;
        private readonly ConcurrentDictionary<string, CosmosClient> _clients;

        public ClientPool(IAccountDirectory accountDirectory)
        {
            _accountDirectory = accountDirectory;
            _clients = new ConcurrentDictionary<string, CosmosClient>();
        }

        public CosmosClient GetClientForAccount(string accountId)
        {
            if (accountId is null) throw new ArgumentNullException(nameof(accountId));

            return _clients.GetOrAdd(accountId, CreateClient);
        }

        public void RemoveClientForAccount(string accountId)
        {
            if (accountId is null) throw new ArgumentNullException(nameof(accountId));

            if (_clients.TryRemove(accountId, out var client))
                client.Dispose();
        }

        private CosmosClient CreateClient(string accountId)
        {
            if (_accountDirectory.TryGetById(accountId, out var account))
            {
                return new CosmosClient(
                    account.Endpoint,
                    account.Key);
            }

            throw new InvalidOperationException("Account not found");
        }
    }
}
