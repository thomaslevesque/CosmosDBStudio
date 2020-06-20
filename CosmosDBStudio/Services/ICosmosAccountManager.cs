using CosmosDBStudio.Model;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface ICosmosAccountManager
    {
        Task<string[]> GetDatabasesAsync(string accountId);
        Task<string[]> GetContainersAsync(string accountId, string databaseId);

        Task CreateDatabaseAsync(string accountId, CosmosDatabase database);
        Task<int?> GetDatabaseThroughputAsync(string accountId, string databaseId);
        Task SetDatabaseThroughputAsync(string accountId, string databaseId, int throughput);
        Task DeleteDatabaseAsync(string accountId, string databaseId);
    }
}
