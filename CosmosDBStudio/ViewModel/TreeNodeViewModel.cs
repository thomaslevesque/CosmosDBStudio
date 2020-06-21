using EssentialMVVM;
using System.Collections.Generic;
using System.Linq;

namespace CosmosDBStudio.ViewModel
{
    public abstract class TreeNodeViewModel : BindableBase
    {
        public abstract string Text { get; }

        public abstract NonLeafTreeNodeViewModel? Parent { get; }

        public virtual IEnumerable<CommandViewModel> Commands => Enumerable.Empty<CommandViewModel>();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value).AndExecute(OnIsExpandedChanged);
        }

        protected virtual void OnIsExpandedChanged()
        {
        }

        private string? _error;
        public string? Error
        {
            get => _error;
            set => Set(ref _error, value).AndNotifyPropertyChanged(nameof(HasError));
        }

        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}