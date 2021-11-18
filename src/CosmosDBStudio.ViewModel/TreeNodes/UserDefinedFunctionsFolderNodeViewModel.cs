using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class UserDefinedFunctionsFolderNodeViewModel : ScriptFolderNodeViewModel
    {
        public UserDefinedFunctionsFolderNodeViewModel(
            IContainerContext containerContext,
            NonLeafTreeNodeViewModel parent,
            ScriptCommands<CosmosUserDefinedFunction> commands,
            IViewModelFactory viewModelFactory)
            : base("User-defined functions", containerContext, parent, viewModelFactory)
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
            var functions = await Context.Scripts.GetUserDefinedFunctionsAsync(default);
            return functions.Select(udf => ViewModelFactory.CreateUserDefinedFunctionNode(udf, Context, this));
        }
    }
}
