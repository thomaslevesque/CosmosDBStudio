using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services.Implementation
{
    public class AccountContext : IAccountContext
    {
        private readonly CosmosAccount _account;
        private readonly CosmosClient _client;

        public AccountContext(CosmosAccount account, CosmosClient client)
        {
            _account = account;
            _client = client;
            Databases = new DatabaseService(client);
        }

        public string AccountId => _account.Id;

        public string AccountName => _account.Name;

        public IDatabaseService Databases { get; }

        public IDatabaseContext GetDatabaseContext(CosmosDatabase database)
        {
            return new DatabaseContext(this, _client.GetDatabase(database.Id));
        }
    }
}
