using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class TriggersFolderViewModel : ContainerScriptFolderViewModel
    {
        public TriggersFolderViewModel(
            ContainerViewModel container,
            ICosmosAccountManager accountManager,
            ScriptCommands<CosmosTrigger> commands,
            IViewModelFactory viewModelFactory)
            : base(container, "Triggers", accountManager, viewModelFactory)
        {
            Commands = new[]
            {
                new CommandViewModel($"New trigger", commands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand)
            };
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var database = Container.Database;
            var account = database.Account;
            var triggers = await AccountManager.GetTriggersAsync(account.Id, database.Id, Container.Id);
            return triggers.Select(t => ViewModelFactory.CreateTriggerNode(Container, this, t));
        }
    }
}
