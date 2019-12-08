using CosmosDBStudio.Model;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services
{
    public interface IQueryService
    {
        Task<QueryResult> ExecuteAsync(Query query, CancellationToken cancellationToken);
    }
}
