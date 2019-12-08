using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class AccountsViewModel : BindableBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IDialogService _dialogService;

        public AccountsViewModel(
            IViewModelFactory viewModelFactory,
            IAccountDirectory accountDirectory,
            IDialogService dialogService)
        {
            _viewModelFactory = viewModelFactory;
            _accountDirectory = accountDirectory;
            _dialogService = dialogService;
            Accounts = new ObservableCollection<AccountViewModel>();
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            foreach (var account in _accountDirectory.Accounts)
            {
                Accounts.Add(_viewModelFactory.CreateAccountViewModel(account));
            }
        }

        public ObservableCollection<AccountViewModel> Accounts { get; }

        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value)
                .AndExecute(() => _editCommand?.RaiseCanExecuteChanged())
                .AndExecute(() => _deleteCommand?.RaiseCanExecuteChanged());
        }

        private DelegateCommand? _addCommand;
        public ICommand AddCommand => _addCommand ??= new DelegateCommand(AddAccount);

        private void AddAccount()
        {
            var dialog = new AccountEditorViewModel();
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var newAccount = new CosmosAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = dialog.Name,
                    Endpoint = dialog.Endpoint,
                    Key = dialog.Key,
                };
                _accountDirectory.Add(newAccount);
                _accountDirectory.Save();

                Accounts.Add(_viewModelFactory.CreateAccountViewModel(newAccount));
            }
        }

        private DelegateCommand? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand(DeleteAccount, CanDeleteAccount);

        private void DeleteAccount()
        {
            if (!(SelectedItem is AccountViewModel accountVm))
                return;

            if (!_dialogService.Confirm($"Are you sure you want to delete account '{accountVm.Name}'?"))
                return;

            _accountDirectory.Remove(accountVm.Id);
            _accountDirectory.Save();

            Accounts.Remove(accountVm);
        }

        private bool CanDeleteAccount() => SelectedItem is AccountViewModel;

        private DelegateCommand? _editCommand;
        public ICommand EditCommand => _editCommand ??= new DelegateCommand(EditAccount, CanEditAccount);

        private void EditAccount()
        {
            if (!(SelectedItem is AccountViewModel accountVm))
                return;

            if (!_accountDirectory.TryGetById(accountVm.Id, out var account))
                return;

            var dialog = new AccountEditorViewModel(account);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                account.Name = dialog.Name;
                account.Endpoint = dialog.Endpoint;
                account.Key = dialog.Key;
                _accountDirectory.Update(account);
                _accountDirectory.Save();

                accountVm.Name = account.Name;
            }
        }

        private bool CanEditAccount() => SelectedItem is AccountViewModel;
    }
}
