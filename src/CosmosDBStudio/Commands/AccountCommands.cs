using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using CosmosDBStudio.ViewModel;
using EssentialMVVM;
using System;
using System.Windows.Input;

namespace CosmosDBStudio.Commands
{
    public class AccountCommands
    {
        private readonly Lazy<IViewModelFactory> _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IMessenger _messenger;

        public AccountCommands(
            Lazy<IViewModelFactory> viewModelFactory,
            IDialogService dialogService,
            IAccountDirectory accountDirectory,
            IMessenger messenger)
        {
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _accountDirectory = accountDirectory;
            _messenger = messenger;
        }

        #region Add

        private DelegateCommand<TreeNodeViewModel?>? _addCommand;
        public ICommand AddCommand => _addCommand ??= new DelegateCommand<TreeNodeViewModel?>(AddAccount, CanAddAccount);

        private void AddAccount(TreeNodeViewModel? parent)
        {
            string folder = parent switch
            {
                AccountFolderViewModel f => f.FullPath,
                AccountViewModel a => (a.Parent as AccountFolderViewModel)?.FullPath ?? string.Empty,
                _ => string.Empty
            };

            var dialog = _viewModelFactory.Value.CreateAccountEditorViewModel();
            dialog.Folder = folder;
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var newAccount = new CosmosAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = dialog.Name,
                    Endpoint = dialog.Endpoint,
                    Key = dialog.Key,
                    Folder = dialog.Folder.Trim('/')
                };
                _accountDirectory.Add(newAccount);
                _accountDirectory.Save();

                _messenger.Publish(new AccountAddedMessage(newAccount));

            }
        }

        private bool CanAddAccount(TreeNodeViewModel? parent) =>
            parent is null ||
            parent is AccountFolderViewModel ||
            parent is AccountViewModel;

        #endregion

        #region Edit

        private DelegateCommand<AccountViewModel>? _editCommand;
        public ICommand EditCommand => _editCommand ??= new DelegateCommand<AccountViewModel>(EditAccount);

        private void EditAccount(AccountViewModel accountVm)
        {
            if (!_accountDirectory.TryGetById(accountVm.Id, out var account))
                return;

            var oldAccount = account.Clone();
            var dialog = _viewModelFactory.Value.CreateAccountEditorViewModel(account);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                account.Name = dialog.Name;
                account.Endpoint = dialog.Endpoint;
                account.Key = dialog.Key;
                account.Folder = dialog.Folder.Trim('/');
                _accountDirectory.Update(account);
                _accountDirectory.Save();

                accountVm.Name = account.Name;

                _messenger.Publish(new AccountEditedMessage(account, oldAccount));
            }
        }

        #endregion

        #region Remove

        private DelegateCommand<AccountViewModel>? _removeCommand;
        public ICommand RemoveCommand => _removeCommand ??= new DelegateCommand<AccountViewModel>(Remove);

        private void Remove(AccountViewModel accountVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to remove account '{accountVm.Name}'?"))
                return;

            if (!_accountDirectory.TryGetById(accountVm.Id, out var account))
                return;

            _accountDirectory.Remove(accountVm.Id);
            _accountDirectory.Save();

            _messenger.Publish(new AccountRemovedMessage(account));
        }

        #endregion
    }
}
