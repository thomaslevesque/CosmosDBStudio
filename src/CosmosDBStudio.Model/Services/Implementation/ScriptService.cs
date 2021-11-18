using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json.Linq;

namespace CosmosDBStudio.Model.Services.Implementation
{
    public class ScriptService : IScriptService
    {
        private readonly Func<Container> _containerGetter;

        public ScriptService(Func<Container> containerGetter)
        {
            _containerGetter = containerGetter;
        }

        public Task<CosmosStoredProcedure[]> GetStoredProceduresAsync(CancellationToken cancellationToken) =>
            GetScripts(() => _containerGetter().Scripts.GetStoredProcedureQueryIterator<StoredProcedureProperties>(), MakeScript, cancellationToken);

        public Task<CosmosUserDefinedFunction[]> GetUserDefinedFunctionsAsync(CancellationToken cancellationToken) =>
            GetScripts(() => _containerGetter().Scripts.GetUserDefinedFunctionQueryIterator<UserDefinedFunctionProperties>(), MakeScript, cancellationToken);

        public Task<CosmosTrigger[]> GetTriggersAsync(CancellationToken cancellationToken) =>
            GetScripts(() => _containerGetter().Scripts.GetTriggerQueryIterator<TriggerProperties>(), MakeScript, cancellationToken);

        public Task<OperationResult> CreateStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken) =>
            CreateScript(storedProcedure, GetProps, _containerGetter().Scripts.CreateStoredProcedureAsync, cancellationToken);

        public Task<OperationResult> CreateTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken) =>
            CreateScript(trigger, GetProps, _containerGetter().Scripts.CreateTriggerAsync, cancellationToken);

        public Task<OperationResult> CreateUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken) =>
            CreateScript(udf, GetProps, _containerGetter().Scripts.CreateUserDefinedFunctionAsync, cancellationToken);

        public Task<OperationResult> ReplaceStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken) =>
            ReplaceScript(storedProcedure, GetProps, _containerGetter().Scripts.ReplaceStoredProcedureAsync, cancellationToken);

        public Task<OperationResult> ReplaceTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken) =>
            ReplaceScript(trigger, GetProps, _containerGetter().Scripts.ReplaceTriggerAsync, cancellationToken);

        public Task<OperationResult> ReplaceUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken) =>
            ReplaceScript(udf, GetProps, _containerGetter().Scripts.ReplaceUserDefinedFunctionAsync, cancellationToken);

        public Task<OperationResult> DeleteStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken) =>
            DeleteScript(storedProcedure, _containerGetter().Scripts.DeleteStoredProcedureAsync, cancellationToken);

        public Task<OperationResult> DeleteUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken) =>
            DeleteScript(udf, _containerGetter().Scripts.DeleteUserDefinedFunctionAsync, cancellationToken);

        public Task<OperationResult> DeleteTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken) =>
            DeleteScript(trigger, _containerGetter().Scripts.DeleteTriggerAsync, cancellationToken);

        private static CosmosStoredProcedure MakeScript(StoredProcedureProperties props) =>
            new CosmosStoredProcedure
            {
                Id = props.Id,
                Body = props.Body,
                ETag = props.ETag
            };

        private static CosmosUserDefinedFunction MakeScript(UserDefinedFunctionProperties props) =>
            new CosmosUserDefinedFunction
            {
                Id = props.Id,
                Body = props.Body,
                ETag = props.ETag
            };

        private static CosmosTrigger MakeScript(TriggerProperties props) =>
            new CosmosTrigger
            {
                Id = props.Id,
                Body = props.Body,
                ETag = props.ETag,
                Operation = props.TriggerOperation,
                Type = props.TriggerType
            };

        private static StoredProcedureProperties GetProps(CosmosStoredProcedure sp) =>
            new StoredProcedureProperties(sp.Id, sp.Body);

        private static TriggerProperties GetProps(CosmosTrigger trigger) =>
            new TriggerProperties
            {
                Id = trigger.Id,
                Body = trigger.Body,
                TriggerOperation = trigger.Operation,
                TriggerType = trigger.Type
            };

        private static UserDefinedFunctionProperties GetProps(CosmosUserDefinedFunction udf) =>
            new UserDefinedFunctionProperties { Id = udf.Id, Body = udf.Body };

        public async Task<StoredProcedureResult> ExecuteStoredProcedureAsync(CosmosStoredProcedure storedProcedure, object? partitionKey, object?[] parameters, CancellationToken cancellationToken)
        {
            var result = new StoredProcedureResult();
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                var response = await _containerGetter().Scripts.ExecuteStoredProcedureAsync<JToken>(
                    storedProcedure.Id,
                    PartitionKeyHelper.Create(partitionKey),
                    parameters,
                    new StoredProcedureRequestOptions { EnableScriptLogging = true },
                    cancellationToken);
                stopwatch.Stop();

                result.Body = response.Resource;
                result.ScriptLog = response.ScriptLog ?? string.Empty;
                result.RequestCharge = response.RequestCharge;
                result.StatusCode = response.StatusCode;
            }
            catch(CosmosException ex)
            {
                result.Error = ex;
                result.StatusCode = ex.StatusCode;
                var scriptLog = ex.Headers["x-ms-documentdb-script-log-results"] ?? string.Empty;
                result.ScriptLog = Uri.UnescapeDataString(scriptLog);
            }
            catch(Exception ex)
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

        private static async Task<TScript[]> GetScripts<TScript, TProperties>(
            Func<FeedIterator<TProperties>> getIterator,
            Func<TProperties, TScript> makeScript,
            CancellationToken cancellationToken)
        {
            var iterator = getIterator();
            var scripts = new List<TScript>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                scripts.AddRange(response.Select(makeScript));
            }

            return scripts.ToArray();
        }

        private static async Task<OperationResult> CreateScript<TScript, TProperties, TResponse>(
            TScript script,
            Func<TScript, TProperties> createProperties,
            Func<TProperties, RequestOptions, CancellationToken, Task<TResponse>> create,
            CancellationToken cancellationToken)
            where TScript : ICosmosScript
            where TResponse : Response<TProperties>
        {
            try
            {
                var props = createProperties(script);
                var options = new RequestOptions();
                var response = await create(props, options, cancellationToken);
                script.ETag = response.ETag;
                return OperationResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return OperationResult.AlreadyExists;
            }
        }

        private static async Task<OperationResult> ReplaceScript<TScript, TProperties, TResponse>(
            TScript script,
            Func<TScript, TProperties> createProperties,
            Func<TProperties, RequestOptions, CancellationToken, Task<TResponse>> replace,
            CancellationToken cancellationToken)
            where TScript : ICosmosScript
            where TResponse : Response<TProperties>
        {
            try
            {
                var props = createProperties(script);
                var options = new RequestOptions { IfMatchEtag = script.ETag };
                var response = await replace(props, options, cancellationToken);
                script.ETag = response.ETag;
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

        private static async Task<OperationResult> DeleteScript<TScript>(
            TScript script,
            Func<string, RequestOptions, CancellationToken, Task> delete,
            CancellationToken cancellationToken)
            where TScript : ICosmosScript
        {
            try
            {
                var options = new RequestOptions { IfMatchEtag = script.ETag };
                await delete(script.Id, options, cancellationToken);
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
