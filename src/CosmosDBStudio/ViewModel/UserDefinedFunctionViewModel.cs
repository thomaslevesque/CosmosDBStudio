using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionViewModel : ContainerScriptViewModel<CosmosUserDefinedFunction>
    {
        public UserDefinedFunctionViewModel(
            ContainerViewModel container,
            NonLeafTreeNodeViewModel parent,
            CosmosUserDefinedFunction udf,
            ScriptCommands<CosmosUserDefinedFunction> commands,
            IMessenger messenger)
            : base(container, parent, udf, commands, messenger)
        {
        }

        public override string Description => "user-defined function";

        public override Task DeleteAsync(ICosmosAccountManager accountManager)
        {
            return accountManager.DeleteUserDefindeFunctionAsync(
                Container.Database.Account.Id,
                Container.Database.Id,
                Container.Id,
                Script);
        }
    }
}
