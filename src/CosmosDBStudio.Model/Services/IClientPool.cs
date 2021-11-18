using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Model.Services
{
    public interface IClientPool
    {
        CosmosClient GetClientForAccount(CosmosAccount account);
        void RemoveClientForAccount(CosmosAccount account);
    }
}
