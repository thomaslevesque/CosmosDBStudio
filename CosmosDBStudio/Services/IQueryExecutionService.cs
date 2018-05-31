using System.Threading.Tasks;
using CosmosDBStudio.Model;

namespace CosmosDBStudio.Services
{
    public interface IQueryExecutionService
    {
        Task<QueryResult> ExecuteAsync(Query query);
    }
}
