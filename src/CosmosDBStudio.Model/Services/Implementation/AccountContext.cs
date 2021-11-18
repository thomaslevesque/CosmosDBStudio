using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System;

namespace CosmosDBStudio.Services.Implementation
{
    public class AccountContext : IAccountContext
    {
        private readonly CosmosAccount _account;
        private readonly Func<CosmosClient> _clientGetter;

        public AccountContext(CosmosAccount account, Func<CosmosClient> clientGetter)
        {
            _account = account;
            _clientGetter = clientGetter;
            Databases = new DatabaseService(clientGetter);
        }

        public string AccountId => _account.Id;

        public string AccountName => _account.Name;

        public bool IsServerless => _account.IsServerless;

        public IDatabaseService Databases { get; }

        public IDatabaseContext GetDatabaseContext(CosmosDatabase database)
        {
            return new DatabaseContext(this, database.Id, () => _clientGetter().GetDatabase(database.Id));
        }
    }
}
