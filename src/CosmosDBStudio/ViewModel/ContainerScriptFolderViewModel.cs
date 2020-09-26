using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ContainerScriptFolderViewModel : NonLeafTreeNodeViewModel
    {
        protected ContainerScriptFolderViewModel(
            ContainerViewModel container,
            string text,
            ICosmosAccountManager accountManager,
            IViewModelFactory viewModelFactory)
        {
            Container = container;
            Text = text;
            AccountManager = accountManager;
            ViewModelFactory = viewModelFactory;
        }

        public ContainerViewModel Container { get; }

        public override string Text { get; }

        public override NonLeafTreeNodeViewModel? Parent => Container;
        protected ICosmosAccountManager AccountManager { get; }
        protected IViewModelFactory ViewModelFactory { get; }
    }
}
