using CosmosDBStudio.Commands;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProceduresFolderViewModel : ContainerScriptFolderViewModel
    {
        public StoredProceduresFolderViewModel(
            ContainerViewModel container,
            ICosmosAccountManager accountManager,
            ScriptCommands<CosmosStoredProcedure> commands,
            IViewModelFactory viewModelFactory)
            : base(container, "Stored procedures", accountManager, viewModelFactory)
        {
            Commands = new[]
            {
                new CommandViewModel($"New stored procedure", commands.CreateCommand, this),
                CommandViewModel.Separator(),
                new CommandViewModel("Refresh", RefreshCommand)
            };
        }

        public override IEnumerable<CommandViewModel> Commands { get; }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var database = Container.Database;
            var account = database.Account;
            var storedProcedures = await AccountManager.GetStoredProceduresAsync(account.Id, database.Id, Container.Id);
            return storedProcedures.Select(sp => ViewModelFactory.CreateStoredProcedure(Container, this, sp));
        }
    }
}
