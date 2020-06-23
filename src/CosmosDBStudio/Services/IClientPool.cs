using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services
{
    public interface IClientPool
    {
        CosmosClient GetClientForAccount(string accountId);
        void RemoveClientForAccount(string accountId);
    }
}
