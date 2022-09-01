using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Func<CosmosClient> _clientGetter;

        public DatabaseService(Func<CosmosClient> clientGetter)
        {
            _clientGetter = clientGetter;
        }

        public async Task<CosmosDatabase[]> GetDatabasesAsync(CancellationToken cancellationToken)
        {
            var iterator = _clientGetter().GetDatabaseQueryIterator<DatabaseProperties>();
            var databases = new List<CosmosDatabase>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                databases.AddRange(response.Select(d => new CosmosDatabase
                {
                    Id = d.Id,
                    ETag = d.ETag
                }));
            }

            return databases.ToArray();
        }

        public async Task<CosmosDatabase> GetDatabaseAsync(string databaseId, CancellationToken cancellationToken)
        {
            var database = _clientGetter().GetDatabase(databaseId);
            var response = await database.ReadAsync(cancellationToken: cancellationToken);

            return new CosmosDatabase
            {
                Id = databaseId,
                ETag = response.Resource.ETag
            };
        }

        public async Task<OperationResult> CreateDatabaseAsync(CosmosDatabase database, int? throughput, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _clientGetter().CreateDatabaseAsync(database.Id, throughput, cancellationToken: cancellationToken);
                database.ETag = response.Resource.ETag;
                return OperationResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return OperationResult.AlreadyExists;
            }
        }

        public async Task<OperationResult> DeleteDatabaseAsync(CosmosDatabase database, CancellationToken cancellationToken)
        {
            var db = _clientGetter().GetDatabase(database.Id);
            try
            {
                await db.DeleteAsync(new RequestOptions { IfMatchEtag = database.ETag }, cancellationToken);
                return OperationResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return OperationResult.EditConflict;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return OperationResult.NotFound;
            }
            catch (CosmosException ex) when (IsTransient(ex.StatusCode))
            {
                try 
                {
                    await db.DeleteAsync(new RequestOptions { IfMatchEtag = database.ETag }, cancellationToken);
                    return OperationResult.Success;
                }
                catch (CosmosException excp) when (excp.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
                {
                    return OperationResult.Success;
                }
            }
        }

        static bool IsTransient(HttpStatusCode statusCode)
            => statusCode == HttpStatusCode.ServiceUnavailable
                || statusCode == HttpStatusCode.TooManyRequests
                || statusCode == HttpStatusCode.RequestTimeout
                || statusCode == HttpStatusCode.Gone;
    }
}
