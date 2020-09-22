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
            IViewModelFactory viewModelFactory)
            : base(container, "Triggers", accountManager, viewModelFactory)
        {
        }

        protected async override Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync()
        {
            var database = Container.Database;
            var account = database.Account;
            var triggers = await AccountManager.GetTriggersAsync(account.Id, database.Id, Container.Id);
            return triggers.Select(t => ViewModelFactory.CreateTrigger(Container, this, t));
        }
    }
}
