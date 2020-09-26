using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionsFolderViewModel : ContainerScriptFolderViewModel
    {
        public UserDefinedFunctionsFolderViewModel(
            ContainerViewModel container,
            ICosmosAccountManager accountManager,
            ScriptCommands<CosmosUserDefinedFunction> commands,
            IViewModelFactory viewModelFactory)
            : base(container, "User-defined functions", accountManager, viewModelFactory)
        {
            Commands = new[]
            {
                new CommandViewModel($"New user-defined function", commands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand),
            };
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var database = Container.Database;
            var account = database.Account;
            var functions = await AccountManager.GetUserDefinedFunctionsAsync(account.Id, database.Id, Container.Id);
            return functions.Select(udf => ViewModelFactory.CreateUserDefinedFunctionNode(Container, this, udf));
        }
    }
}
