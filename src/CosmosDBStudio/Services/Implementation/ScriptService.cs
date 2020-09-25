using CosmosDBStudio.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDBStudio.Services.Implementation
{
    public class ScriptService : IScriptService
    {
        private readonly Container _container;

        public ScriptService(Container container)
        {
            _container = container;
        }

        public Task<ScriptSaveResult> CreateStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken) =>
            CreateScript(storedProcedure, GetProps, _container.Scripts.CreateStoredProcedureAsync, cancellationToken);

        public Task<ScriptSaveResult> CreateTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken) =>
            CreateScript(trigger, GetProps, _container.Scripts.CreateTriggerAsync, cancellationToken);

        public Task<ScriptSaveResult> CreateUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken) =>
            CreateScript(udf, GetProps, _container.Scripts.CreateUserDefinedFunctionAsync, cancellationToken);

        public Task<ScriptSaveResult> ReplaceStoredProcedureAsync(CosmosStoredProcedure storedProcedure, CancellationToken cancellationToken) =>
            ReplaceScript(storedProcedure, GetProps, _container.Scripts.ReplaceStoredProcedureAsync, cancellationToken);

        public Task<ScriptSaveResult> ReplaceTriggerAsync(CosmosTrigger trigger, CancellationToken cancellationToken) =>
            ReplaceScript(trigger, GetProps, _container.Scripts.ReplaceTriggerAsync, cancellationToken);

        public Task<ScriptSaveResult> ReplaceUserDefinedFunctionAsync(CosmosUserDefinedFunction udf, CancellationToken cancellationToken) =>
            ReplaceScript(udf, GetProps, _container.Scripts.ReplaceUserDefinedFunctionAsync, cancellationToken);

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

        private static async Task<ScriptSaveResult> CreateScript<TScript, TProperties, TResponse>(
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
                return ScriptSaveResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return ScriptSaveResult.AlreadyExists;
            }
        }

        private static async Task<ScriptSaveResult> ReplaceScript<TScript, TProperties, TResponse>(
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
                return ScriptSaveResult.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return ScriptSaveResult.EditConflict;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return ScriptSaveResult.NotFound;
            }
        }
    }
}
