using CosmosDBStudio.Model;

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
            var client = _clientPool.GetClientForAccount(account);
            return new AccountContext(account, client);
        }
    }
}
