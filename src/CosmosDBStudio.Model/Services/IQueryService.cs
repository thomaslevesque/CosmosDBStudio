using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Model.Services
{
    public interface IQueryService
    {
        Task<QueryResult> ExecuteAsync(Query query, string? continuationToken, CancellationToken cancellationToken);
    }
}
