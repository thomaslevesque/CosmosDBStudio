using EssentialMVVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class NonLeafTreeNodeViewModel : TreeNodeViewModel
    {
        protected NonLeafTreeNodeViewModel()
        {
            _placeholderChildren = new[] { new PlaceholderTreeNodeViewModel(this) };
        }

        private readonly IEnumerable<TreeNodeViewModel> _placeholderChildren;
        private IEnumerable<TreeNodeViewModel>? _children;

        public IEnumerable<TreeNodeViewModel> Children
        {
            get
            {
                if (IsExpanded && _children == null && _loadChildrenTask == null)
                {
                    DoLoadChildrenAsync();
                }

                return _children ?? _placeholderChildren;
            }
        }

        public async Task EnsureChildrenLoadedAsync()
        {
            if (_children == null)
            {
                await (_loadChildrenTask ?? DoLoadChildrenAsync());
            }
        }

        private Task DoLoadChildrenAsync()
        {
            return _loadChildrenTask = InternalLoadChildrenAsync();
        }

        protected override void OnIsExpandedChanged()
        {
            base.OnIsExpandedChanged();
            if (IsExpanded)
                OnPropertyChanged(nameof(Children));
        }

        private Task? _loadChildrenTask;

        private async Task InternalLoadChildrenAsync()
        {
            try
            {
                Error = null;
                _children = await Task.Run(LoadChildrenAsync);
            }
            catch (Exception ex)
            {
                Error = "Error " + ex.Message;
                _children = new[] { new ErrorTreeNodeViewModel(this, ex) };
            }

            _loadChildrenTask = null;
            OnPropertyChanged(nameof(Children));
        }

        protected abstract Task<IEnumerable<TreeNodeViewModel>> LoadChildrenAsync();

        public void ReloadChildren()
        {
            _children = null;
            OnPropertyChanged(nameof(Children));
        }

        private DelegateCommand? _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new DelegateCommand(ReloadChildren);
    }

    public sealed class PlaceholderTreeNodeViewModel : TreeNodeViewModel
    {
        public PlaceholderTreeNodeViewModel(NonLeafTreeNodeViewModel parent)
        {
            Parent = parent;
        }

        public override string Text => "Loading...";
        public override NonLeafTreeNodeViewModel? Parent { get; }
    }

    public sealed class ErrorTreeNodeViewModel : TreeNodeViewModel
    {
        public ErrorTreeNodeViewModel(NonLeafTreeNodeViewModel parent, Exception exception)
        {
            Parent = parent;
            Text = "Error: " + exception.Message;
        }

        public override string Text { get; }
        public override NonLeafTreeNodeViewModel? Parent { get; }

        private DelegateCommand? _retryCommand;
        public ICommand RetryCommand => _retryCommand ??= new DelegateCommand(Retry);

        private void Retry()
        {
            Parent?.ReloadChildren();
        }
    }
}