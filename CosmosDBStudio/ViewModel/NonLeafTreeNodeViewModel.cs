using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public abstract class TreeNodeViewModel : BindableBase
    {
        public abstract string Text { get; }

        public virtual IEnumerable<MenuCommandViewModel> MenuCommands => Enumerable.Empty<MenuCommandViewModel>();
    }

    public abstract class NonLeafTreeNodeViewModel : TreeNodeViewModel
    {
        protected NonLeafTreeNodeViewModel()
        {
            _placeholderChildren = new[] { new PlaceholderTreeNodeViewModel() };
        }

        private readonly IEnumerable<TreeNodeViewModel> _placeholderChildren;
        private IEnumerable<TreeNodeViewModel> _children;

        public IEnumerable<TreeNodeViewModel> Children
        {
            get
            {
                if (_children == null && _loadChildrenTask == null)
                {
                    _loadChildrenTask = InternalLoadChildrenAsync();
                }

                return _children ?? _placeholderChildren;
            }
        }

        private Task _loadChildrenTask;

        private async Task InternalLoadChildrenAsync()
        {
            _children = await LoadChildrenAsync();
            _loadChildrenTask = null;
            OnPropertyChanged(nameof(Children));
        }

        protected abstract Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync();
    }

    sealed class PlaceholderTreeNodeViewModel : TreeNodeViewModel
    {
        public override string Text => "Loading...";
    }
}