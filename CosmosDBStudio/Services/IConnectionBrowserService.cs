using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IConnectionBrowserService
    {
        Task<string[]> GetDatabasesAsync(string connectionId);
        Task<string[]> GetContainersAsync(string connectionId, string databaseId);
    }
}
