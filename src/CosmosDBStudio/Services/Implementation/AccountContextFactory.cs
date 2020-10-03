using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System;

namespace CosmosDBStudio.Services.Implementation
{
    public class AccountContextFactory : IAccountContextFactory
    {
        private readonly IClientPool _clientPool;

        public AccountContextFactory(IClientPool clientPool)
        {
            _clientPool = clientPool;
        }

        public IAccountContext Create(CosmosAccount account)
        {
            return new AccountContext(account, new Lazy<CosmosClient>(() => _clientPool.GetClientForAccount(account)));
        }
    }
}
