using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Services.Implementation
{
    public class QueryService : IQueryService
    {
        private readonly Container _container;

        public QueryService(Container container)
        {
            _container = container;
        }

        public async Task<QueryResult> ExecuteAsync(Query query, CancellationToken cancellationToken)
        {
            var queryDefinition = CreateQueryDefinition(query);
            var requestOptions = CreateRequestOptions(query);
            var iterator = _container.GetItemQueryIterator<JToken>(queryDefinition, query.ContinuationToken, requestOptions);
            var result = new QueryResult();
            var stopwatch = new Stopwatch();
            List<string>? warnings = null;
            try
            {
                stopwatch.Start();
                var response = await iterator.ReadNextAsync();
                stopwatch.Stop();
                result.RequestCharge += response.RequestCharge;
                try
                {
                    result.ContinuationToken = response.ContinuationToken;
                }
                catch (Exception ex)
                {
                    warnings ??= new List<string>();
                    warnings.Add(ex.Message);
                }

                result.Items = response.ToList();
                if (warnings != null)
                    result.Warnings = warnings;
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

        private static QueryDefinition CreateQueryDefinition(Query query)
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

        private static QueryRequestOptions CreateRequestOptions(Query query)
        {
            return new QueryRequestOptionsBuilder()
                .WithPartitionKey(query.PartitionKey)
                .WithMaxItemCount(100)
                .Build();
        }
    }
}
