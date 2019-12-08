using System.Collections.Generic;
using System.Linq;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public abstract class TreeNodeViewModel : BindableBase
    {
        public abstract string Text { get; }

        public abstract NonLeafTreeNodeViewModel? Parent { get; }

        public virtual IEnumerable<MenuCommandViewModel> MenuCommands => Enumerable.Empty<MenuCommandViewModel>();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value).AndExecute(OnIsExpandedChanged);
        }

        protected virtual void OnIsExpandedChanged()
        {
        }
    }
}