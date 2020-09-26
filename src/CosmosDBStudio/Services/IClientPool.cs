using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Services
{
    public interface IClientPool
    {
        CosmosClient GetClientForAccount(CosmosAccount account);
        void RemoveClientForAccount(CosmosAccount account);
    }
}
