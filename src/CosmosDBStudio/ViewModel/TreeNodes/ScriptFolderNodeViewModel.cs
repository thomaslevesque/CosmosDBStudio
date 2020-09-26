using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ScriptFolderNodeViewModel : NonLeafTreeNodeViewModel
    {
        protected ScriptFolderNodeViewModel(
            ContainerNodeViewModel container,
            string text,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory)
        {
            Container = container;
            Text = text;
            AccountManager = accountManager;
            ViewModelFactory = viewModelFactory;
        }

        public ContainerNodeViewModel Container { get; }

        public override string Text { get; }

        public override NonLeafTreeNodeViewModel? Parent => Container;
        protected ICosmosAccountManager AccountManager { get; }
        protected IViewModelFactory ViewModelFactory { get; }
    }
}
