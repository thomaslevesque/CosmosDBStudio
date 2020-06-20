using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        private readonly ICosmosAccountManager _accountManager;
        private readonly IDialogService _dialogService;

        public AccountsViewModel(
            IViewModelFactory viewModelFactory,
            IAccountDirectory accountDirectory,
            ICosmosAccountManager accountManager,
            IDialogService dialogService)
        {
            _viewModelFactory = viewModelFactory;
            _accountDirectory = accountDirectory;
            _accountManager = accountManager;
            _dialogService = dialogService;
            RootNodes = new ObservableCollection<TreeNodeViewModel>();
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            var nodes = _accountDirectory.GetRootNodes();
            foreach (var node in nodes)
            {
                var vm = node switch
                {
                    CosmosAccount account => (TreeNodeViewModel)_viewModelFactory.CreateAccountViewModel(account, null),
                    CosmosAccountFolder folder => (TreeNodeViewModel)_viewModelFactory.CreateAccountFolderViewModel(folder, null),
                    _ => throw new Exception("Invalid node type")
                };

                RootNodes.Add(vm);
            }
        }

        public ObservableCollection<TreeNodeViewModel> RootNodes { get; }

        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value)
                .AndRaiseCanExecuteChanged(_editCommand)
                .AndRaiseCanExecuteChanged(_deleteCommand);
        }

        private DelegateCommand? _addAccountCommand;
        public ICommand AddAccountCommand => _addAccountCommand ??= new DelegateCommand(AddAccount, CanAddAccount);

        private bool CanAddAccount() => SelectedItem is AccountFolderViewModel || SelectedItem is AccountViewModel;

        private void AddAccount()
        {
            string folder = SelectedItem switch
            {
                AccountFolderViewModel f => f.FullPath,
                AccountViewModel a => (a.Parent as AccountFolderViewModel)?.FullPath ?? string.Empty,
                _ => string.Empty
            };

            var dialog = _viewModelFactory.CreateAccountEditorViewModel();
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

                var folderVM = GetFolder(newAccount.Folder, create: true);
                var vm = _viewModelFactory.CreateAccountViewModel(newAccount, folderVM);

                if (folderVM is null)
                    RootNodes.Add(vm);
                else
                    folderVM.ReloadChildren();
            }
        }

        private AsyncDelegateCommand? _addDatabaseCommand;
        public ICommand AddDatabaseCommand => _addDatabaseCommand ??= new AsyncDelegateCommand(AddDatabaseAsync, CanAddDatabase);

        private bool CanAddDatabase() => SelectedItem is AccountViewModel;

        private async Task AddDatabaseAsync()
        {
            if (SelectedItem is AccountViewModel account)
            {
                var dialog = _viewModelFactory.CreateDatabaseEditorViewModel();
                if (_dialogService.ShowDialog(dialog) is true)
                {
                    var database = new CosmosDatabase
                    {
                        Id = dialog.Id,
                        Throughput = dialog.ProvisionThroughput
                            ? dialog.Throughput
                            : default(int?)
                    };

                    await _accountManager.CreateDatabaseAsync(account.Id, database);
                    account.ReloadChildren();
                }
            }
        }

        private AsyncDelegateCommand? _addContainerCommand;
        public ICommand AddContainerCommand => _addContainerCommand ??= new AsyncDelegateCommand(AddContainerAsync, CanAddContainer);

        private bool CanAddContainer() => SelectedItem is DatabaseViewModel;

        private async Task AddContainerAsync()
        {
            if (SelectedItem is DatabaseViewModel databaseVm)
            {
                var database = await _accountManager.GetDatabaseAsync(databaseVm.Account.Id, databaseVm.Id);
                var dialog = _viewModelFactory.CreateContainerEditorViewModel(database.Throughput.HasValue);
                if (_dialogService.ShowDialog(dialog) is true)
                {
                    var container = dialog.GetContainer();
                    await _accountManager.CreateContainerAsync(databaseVm.Account.Id, databaseVm.Id, container);
                    databaseVm.ReloadChildren();
                }
            }
        }

        private AsyncDelegateCommand? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand(DeleteAsync, CanDelete);

        private async Task DeleteAsync()
        {
            switch (SelectedItem)
            {
                case AccountViewModel accountVm:
                    DeleteAccount(accountVm);
                    break;
                case DatabaseViewModel databaseVm:
                    await DeleteDatabaseAsync(databaseVm);
                    break;
                case ContainerViewModel containerVm:
                    await DeleteContainerAsync(containerVm);
                    break;
            }
        }

        private void DeleteAccount(AccountViewModel accountVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to remove account '{accountVm.Name}'?"))
                return;

            _accountDirectory.Remove(accountVm.Id);
            _accountDirectory.Save();

            var folder = accountVm.Parent;
            if (folder is null)
                RootNodes.Remove(accountVm);
            else
                folder.ReloadChildren();
        }

        private async Task DeleteDatabaseAsync(DatabaseViewModel databaseVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete database '{databaseVm.Id}'?"))
                return;

            var accountVm = databaseVm.Account;
            await _accountManager.DeleteDatabaseAsync(accountVm.Id, databaseVm.Id);
            accountVm.ReloadChildren();
        }

        private async Task DeleteContainerAsync(ContainerViewModel containerVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete container '{containerVm.Id}'?"))
                return;

            var databaseVm = containerVm.Database;
            var accountVm = databaseVm.Account;
            await _accountManager.DeleteContainerAsync(accountVm.Id, databaseVm.Id, containerVm.Id);
            databaseVm.ReloadChildren();
        }

        private bool CanDelete() =>
            SelectedItem is AccountViewModel ||
            SelectedItem is DatabaseViewModel ||
            SelectedItem is ContainerViewModel;

        private DelegateCommand? _editCommand;
        public ICommand EditCommand => _editCommand ??= new DelegateCommand(EditAccount, CanEditAccount);

        private void EditAccount()
        {
            if (!(SelectedItem is AccountViewModel accountVm))
                return;

            if (!_accountDirectory.TryGetById(accountVm.Id, out var account))
                return;

            var oldFolder = account.Folder;
            var dialog = _viewModelFactory.CreateAccountEditorViewModel(account);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                account.Name = dialog.Name;
                account.Endpoint = dialog.Endpoint;
                account.Key = dialog.Key;
                account.Folder = dialog.Folder.Trim('/');
                _accountDirectory.Update(account);
                _accountDirectory.Save();

                accountVm.Name = account.Name;

                if (account.Folder != oldFolder)
                {
                    ReloadAccounts();
                }
            }
        }

        private void ReloadAccounts()
        {
            RootNodes.Clear();
            LoadAccounts();
        }

        private bool CanEditAccount() => SelectedItem is AccountViewModel;

        private AccountFolderViewModel? GetFolder(string folder, bool create)
        {
            folder = folder.Trim('/');
            if (string.IsNullOrEmpty(folder))
                return null;

            var parts = folder.Trim('/').Split('/');
            AccountFolderViewModel? currentFolderVM = null;
            string currentPath = "";
            foreach (var name in parts)
            {
                currentPath = string.IsNullOrEmpty(currentPath)
                    ? name
                    : currentPath + "/" + name;

                AccountFolderViewModel? nextFolderVM = null;
                if (currentFolderVM is null)
                {
                    nextFolderVM = RootNodes.OfType<AccountFolderViewModel>().FirstOrDefault(f => f.FullPath == currentPath);
                }
                else
                {
                    // Known to be synchronous for folders
                    currentFolderVM.EnsureChildrenLoadedAsync().Wait();
                    nextFolderVM = currentFolderVM.Children.OfType<AccountFolderViewModel>().FirstOrDefault(f => f.FullPath == currentPath);
                }

                if (nextFolderVM is null)
                {
                    if (!create)
                        return null;

                    var newFolder = new CosmosAccountFolder(currentPath);
                    nextFolderVM = _viewModelFactory.CreateAccountFolderViewModel(newFolder, currentFolderVM);
                }

                currentFolderVM = nextFolderVM;
            }

            return currentFolderVM;
        }
    }
}
