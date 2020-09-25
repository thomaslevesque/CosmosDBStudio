using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionEditorViewModel : ScriptEditorViewModelBase<CosmosUserDefinedFunction>
    {
        public UserDefinedFunctionEditorViewModel(IContainerContext containerContext, CosmosUserDefinedFunction udf)
            : base(containerContext, udf)
        {
        }

        public override string Description => "user-defined function";

        protected override Task CreateScriptAsync(CosmosUserDefinedFunction script) =>
            ContainerContext.Scripts.CreateUserDefinedFunctionAsync(script, default);

        protected override Task ReplaceScriptAsync(CosmosUserDefinedFunction script) =>
            ContainerContext.Scripts.ReplaceUserDefinedFunctionAsync(script, default);
    }
}
