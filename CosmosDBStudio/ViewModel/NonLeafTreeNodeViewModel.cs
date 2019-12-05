using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
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
                if (IsExpanded && _children == null && _loadChildrenTask == null)
                {
                    _loadChildrenTask = InternalLoadChildrenAsync();
                }

                return _children ?? _placeholderChildren;
            }
        }

        protected override void OnIsExpandedChanged()
        {
            base.OnIsExpandedChanged();
            if (IsExpanded)
                OnPropertyChanged(nameof(Children));
        }

        private string _error;
        public string Error
        {
            get => _error;
            set => Set(ref _error, value).AndNotifyPropertyChanged(nameof(HasError));
        }

        public bool HasError => !string.IsNullOrEmpty(Error);

        private Task _loadChildrenTask;

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

        public void RetryLoadChildren()
        {
            _children = null;
            OnPropertyChanged(nameof(Children));
        }
    }

    sealed class PlaceholderTreeNodeViewModel : TreeNodeViewModel
    {
        public override string Text => "Loading...";
    }

    sealed class ErrorTreeNodeViewModel : TreeNodeViewModel
    {
        private readonly NonLeafTreeNodeViewModel _parent;
        private readonly Exception _exception;

        public ErrorTreeNodeViewModel(NonLeafTreeNodeViewModel parent, Exception exception)
        {
            _parent = parent;
            _exception = exception;
        }

        public override string Text => "Error: " + _exception.Message;

        private DelegateCommand _retryCommand;
        public ICommand RetryCommand => _retryCommand ??= new DelegateCommand(Retry);

        private void Retry()
        {
            _parent.RetryLoadChildren();
        }
    }
}