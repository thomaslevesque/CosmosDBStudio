using CosmosDBStudio.Model.Services;

namespace CosmosDBStudio.ViewModel.TreeNodes
{
    public abstract class ScriptFolderNodeViewModel : NonLeafTreeNodeViewModel
    {
        protected ScriptFolderNodeViewModel(
            string text,
            IContainerContext context,
            NonLeafTreeNodeViewModel parent,
            IViewModelFactory viewModelFactory)
        {
            Parent = parent;
            Text = text;
            Context = context;
            ViewModelFactory = viewModelFactory;
        }

        public override string Text { get; }
        public IContainerContext Context { get; }

        public override NonLeafTreeNodeViewModel? Parent { get; }
        protected IViewModelFactory ViewModelFactory { get; }
    }
}
