namespace CosmosDBStudio.Model.Services.Implementation
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
            return new AccountContext(account, () => _clientPool.GetClientForAccount(account));
        }
    }
}
