using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Services.Implementation
{
    public class QueryExecutionService : IQueryExecutionService
    {
        private readonly IAccountDirectory _accountDirectory;

        public QueryExecutionService(IAccountDirectory accountDirectory)
        {
            _accountDirectory = accountDirectory;
        }

        public async Task<QueryResult> ExecuteAsync(Query query)
        {
            if (string.IsNullOrEmpty(query.AccountId))
            {
                throw new ArgumentException("No account specified");
            }

            if (string.IsNullOrEmpty(query.DatabaseId))
            {
                throw new ArgumentException("No database specified");
            }

            if (string.IsNullOrEmpty(query.ContainerId))
            {
                throw new ArgumentException("No container specified");
            }

            var account = _accountDirectory.GetById(query.AccountId);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found");
            }

            using var client = CreateCosmosClient(account);
            var container = client.GetContainer(query.DatabaseId, query.ContainerId);
            var queryDefinition = CreateQueryDefinition(query);
            var requestOptions = CreateRequestOptions(query.Options);
            var iterator = container.GetItemQueryIterator<JObject>(queryDefinition, query.ContinuationToken, requestOptions);
            var result = new QueryResult();
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                var response = await iterator.ReadNextAsync();
                stopwatch.Stop();
                result.RequestCharge += response.RequestCharge;
                result.ContinuationToken = response.ContinuationToken;
                result.Documents = response.ToList();
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }
            finally
            {
                stopwatch.Stop();
            }

            result.TimeElapsed = stopwatch.Elapsed;
            return result;
        }

        private CosmosClient CreateCosmosClient(CosmosAccount account)
        {
            // TODO: connection options
            return new CosmosClient(
                account.Endpoint,
                account.Key);
        }

        private QueryDefinition CreateQueryDefinition(Query query)
        {
            var definition = new QueryDefinition(query.Sql);
            if (query.Parameters != null)
            {
                foreach (var (key, value) in query.Parameters)
                {
                    definition = definition.WithParameter(key, value);
                }
            }

            return definition;
        }

        private QueryRequestOptions CreateRequestOptions(QueryOptions options)
        {
            return new QueryRequestOptions
            {
                PartitionKey = options?.PartitionKey switch
                {
                    null => default(PartitionKey?),
                    string s => new PartitionKey(s),
                    double d => new PartitionKey(d),
                    bool b => new PartitionKey(b),
                    _ => throw new ArgumentException("Invalid partition key type")
                },
                MaxItemCount = options?.MaxItemCount ?? 100
            };
        }
    }
}