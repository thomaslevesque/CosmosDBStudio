using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Lazy<CosmosClient> _client;

        public DatabaseService(Lazy<CosmosClient> client)
        {
            _client = client;
        }

        public async Task<CosmosDatabase[]> GetDatabasesAsync(CancellationToken cancellationToken)
        {
            var iterator = _client.Value.GetDatabaseQueryIterator<DatabaseProperties>();
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
            var database = _client.Value.GetDatabase(databaseId);
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
                var response = await _client.Value.CreateDatabaseAsync(database.Id, throughput, cancellationToken: cancellationToken);
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
            var db = _client.Value.GetDatabase(database.Id);
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
