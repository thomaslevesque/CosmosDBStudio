using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class DatabaseService : IDatabaseService
    {
        private readonly CosmosClient _client;

        public DatabaseService(CosmosClient client)
        {
            _client = client;
        }

        public async Task<string[]> GetDatabaseNamesAsync(CancellationToken cancellationToken)
        {
            var iterator = _client.GetDatabaseQueryIterator<DatabaseProperties>();
            var databases = new List<string>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                databases.AddRange(response.Select(d => d.Id));
            }

            return databases.ToArray();
        }

        public async Task<CosmosDatabase> GetDatabaseAsync(string databaseId, CancellationToken cancellationToken)
        {
            var database = _client.GetDatabase(databaseId);
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
                var response = await _client.CreateDatabaseAsync(database.Id, throughput, cancellationToken: cancellationToken);
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
            var db = _client.GetDatabase(database.Id);
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
        }
    }
}
