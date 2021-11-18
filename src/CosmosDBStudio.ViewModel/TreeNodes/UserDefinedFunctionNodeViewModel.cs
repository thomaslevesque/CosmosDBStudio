using System.Threading.Tasks;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;

namespace CosmosDBStudio.ViewModel.TreeNodes
{
    public class UserDefinedFunctionNodeViewModel : ScriptNodeViewModel<CosmosUserDefinedFunction>
    {
        public UserDefinedFunctionNodeViewModel(
            CosmosUserDefinedFunction udf,
            IContainerContext context,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<CosmosUserDefinedFunction> commands,
            IMessenger messenger)
            : base(udf, context, parent, commands, messenger)
        {
        }

        public override string Description => "user-defined function";

        public override Task DeleteAsync() => Context.Scripts.DeleteUserDefinedFunctionAsync(Script, default);
    }
}
