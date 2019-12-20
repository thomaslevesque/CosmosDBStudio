using CosmosDBStudio.Model;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IQueryPersistenceService
    {
        Task<QuerySheet> LoadAsync(string path);
        Task SaveAsync(QuerySheet querySheet, string path);
    }
}
