using EssentialMVVM;
using System;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class TabViewModelBase : BindableBase
    {
        public abstract string Title { get; }

        public abstract string Description { get; }

        private DelegateCommand? _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new DelegateCommand(Close);

        private void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CloseRequested;
    }
}
