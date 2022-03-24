using CosmosDBStudio.Model;
using EssentialMVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;
using CosmosDBStudio.ViewModel.Messages;
using CosmosDBStudio.ViewModel.TreeNodes;

namespace CosmosDBStudio.ViewModel
{
    public class AccountsViewModel : BindableBase
    {
        private readonly AccountCommands _accountCommands;
        private readonly IAccountContextFactory _accountContextFactory;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IAccountDirectory _accountDirectory;
        private readonly IMessenger _messenger;

        public AccountsViewModel(
            AccountCommands accountCommands,
            IAccountContextFactory accountContextFactory,
            IViewModelFactory viewModelFactory,
            IAccountDirectory accountDirectory,
            IMessenger messenger)
        {
            _accountCommands = accountCommands;
            _accountContextFactory = accountContextFactory;
            _viewModelFactory = viewModelFactory;
            _accountDirectory = accountDirectory;
            _messenger = messenger;
            RootNodes = new ObservableCollection<TreeNodeViewModel>();
            LoadAccounts();

            _messenger.Subscribe(this).To<AccountAddedMessage>((vm, message) => vm.OnAccountAdded(message));
            _messenger.Subscribe(this).To<AccountEditedMessage>((vm, message) => vm.OnAccountEdited(message));
            _messenger.Subscribe(this).To<AccountRemovedMessage>((vm, message) => vm.OnAccountRemoved(message));

            Commands = new[]
            {
                new CommandViewModel("Add account", _accountCommands.AddCommand, null)
            };
        }

        public IEnumerable<CommandViewModel> Commands { get; }

        private void LoadAccounts()
        {
            var nodes = _accountDirectory.GetRootNodes();
            foreach (var node in nodes.OrderBy(n => n.DisplayName))
            {
                var vm = node switch
                {
                    CosmosAccount account =>
                        (TreeNodeViewModel)_viewModelFactory.CreateAccountNode(
                            account,
                            _accountContextFactory.Create(account),
                            null),
                    CosmosAccountFolder folder => (TreeNodeViewModel)_viewModelFactory.CreateAccountFolderNode(folder, null),
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
                .AndExecute(() =>
                {
                    var container = GetSelectedContainer();
                    _messenger.Publish(new ExplorerSelectedContainerChangedMessage(container));
                });
        }

        private ContainerNodeViewModel? GetSelectedContainer()
        {
            var node = SelectedItem as TreeNodeViewModel;
            while (node != null)
            {
                if (node is ContainerNodeViewModel vm)
                    return vm;
                node = node.Parent;
            }

            return null;
        }

        private void ReloadAccounts()
        {
            RootNodes.Clear();
            LoadAccounts();
        }

        private AccountFolderNodeViewModel? GetFolder(string folder, bool create)
        {
            folder = folder.Trim('/');
            if (string.IsNullOrEmpty(folder))
                return null;

            var parts = folder.Trim('/').Split('/');
            AccountFolderNodeViewModel? currentFolderVM = null;
            string currentPath = "";
            foreach (var name in parts)
            {
                currentPath = string.IsNullOrEmpty(currentPath)
                    ? name
                    : currentPath + "/" + name;

                AccountFolderNodeViewModel? nextFolderVM = null;
                if (currentFolderVM is null)
                {
                    nextFolderVM = RootNodes.OfType<AccountFolderNodeViewModel>().FirstOrDefault(f => f.FullPath == currentPath);
                }
                else
                {
                    // Known to be synchronous for folders
                    currentFolderVM.EnsureChildrenLoadedAsync().Wait();
                    nextFolderVM = currentFolderVM.Children.OfType<AccountFolderNodeViewModel>().FirstOrDefault(f => f.FullPath == currentPath);
                }

                if (nextFolderVM is null)
                {
                    if (!create)
                        return null;

                    var newFolder = new CosmosAccountFolder(currentPath);
                    nextFolderVM = _viewModelFactory.CreateAccountFolderNode(newFolder, currentFolderVM);
                }

                currentFolderVM = nextFolderVM;
            }

            return currentFolderVM;
        }

        private void OnAccountAdded(AccountAddedMessage message)
        {
            var folderVM = GetFolder(message.Account.Folder, create: true);
            var vm = _viewModelFactory.CreateAccountNode(
                message.Account,
                _accountContextFactory.Create(message.Account),
                folderVM);

            if (folderVM is null)
                RootNodes.Add(vm);
            else
                folderVM.ReloadChildren();
        }

        private void OnAccountEdited(AccountEditedMessage message)
        {
            if (message.Account.Folder != message.OldAccount.Folder)
            {
                // TODO: improve this
                ReloadAccounts();
            }
        }

        private void OnAccountRemoved(AccountRemovedMessage message)
        {
            var folderVm = GetFolder(message.Account.Folder, create: false);
            if (folderVm is null)
            {
                var vm = RootNodes.OfType<AccountNodeViewModel>().FirstOrDefault(vm => vm.Id == message.Account.Id);
                if (vm != null)
                    RootNodes.Remove(vm);
            }
            else
            {
                // TODO: improve this
                folderVm.ReloadChildren();
            }
        }
    }
}
