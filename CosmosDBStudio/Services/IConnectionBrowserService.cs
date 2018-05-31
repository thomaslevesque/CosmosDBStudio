using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IConnectionBrowserService
    {
        Task<string[]> GetDatabasesAsync(string connectionId);
        Task<string[]> GetCollectionsAsync(string connectionId, string databaseId);
    }
}
