using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;

namespace CosmosDBStudio.ViewModel.EditorTabs
{
    public class UserDefinedFunctionEditorViewModel : ScriptEditorViewModelBase<CosmosUserDefinedFunction>
    {
        public UserDefinedFunctionEditorViewModel(CosmosUserDefinedFunction udf, IContainerContext containerContext)
            : base(udf, containerContext)
        {
        }

        public override string Description => "user-defined function";

        protected override Task CreateScriptAsync(CosmosUserDefinedFunction script) =>
            ContainerContext.Scripts.CreateUserDefinedFunctionAsync(script, default);

        protected override Task ReplaceScriptAsync(CosmosUserDefinedFunction script) =>
            ContainerContext.Scripts.ReplaceUserDefinedFunctionAsync(script, default);
    }
}
