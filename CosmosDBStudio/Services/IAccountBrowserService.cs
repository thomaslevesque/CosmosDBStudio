using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IAccountBrowserService
    {
        Task<string[]> GetDatabasesAsync(string accountId);
        Task<string[]> GetContainersAsync(string accountId, string databaseId);
    }
}
