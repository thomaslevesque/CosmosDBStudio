using CosmosDBStudio.Messages;
using CosmosDBStudio.Services;
using CosmosDBStudio.ViewModel;
using EssentialMVVM;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.Commands
{
    public class ContainerCommands
    {
        public ContainerCommands(
            Lazy<IViewModelFactory> viewModelFactory,
            IDialogService dialogService,
            ICosmosAccountManager accountManager,
            IMessenger messenger)
        {
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _accountManager = accountManager;
            _messenger = messenger;
        }

        #region Create

        private AsyncDelegateCommand<DatabaseViewModel>? _createCommand;
        private readonly Lazy<IViewModelFactory> _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly ICosmosAccountManager _accountManager;
        private readonly IMessenger _messenger;

        public ICommand CreateCommand => _createCommand ??= new AsyncDelegateCommand<DatabaseViewModel>(CreateAsync);

        private async Task CreateAsync(DatabaseViewModel databaseVm)
        {
            var accountId = databaseVm.Account.Id;
            var databaseId = databaseVm.Id;
            var database = await _accountManager.GetDatabaseAsync(accountId, databaseId);
            var dialog = _viewModelFactory.Value.CreateContainerEditor(null, database.Throughput.HasValue);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var container = dialog.GetContainer();
                await _accountManager.CreateContainerAsync(accountId, databaseId, container);
                _messenger.Publish(new ContainerCreatedMessage(accountId, databaseId, container));
            }
        }

        #endregion

        #region Edit

        private AsyncDelegateCommand<ContainerViewModel>? _editCommand;
        public ICommand EditCommand => _editCommand ??= new AsyncDelegateCommand<ContainerViewModel>(EditAsync);

        private async Task EditAsync(ContainerViewModel containerVm)
        {
            var accountId = containerVm.Database.Account.Id;
            var databaseId = containerVm.Database.Id;
            var containerId = containerVm.Id;

            var database = await _accountManager.GetDatabaseAsync(accountId, databaseId);
            var container = await _accountManager.GetContainerAsync(accountId, databaseId, containerId);
            var dialog = _viewModelFactory.Value.CreateContainerEditor(container, database.Throughput.HasValue);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                container = dialog.GetContainer();
                await _accountManager.UpdateContainerAsync(accountId, databaseId, container);
            }
        }

        #endregion

        #region Delete

        private AsyncDelegateCommand<ContainerViewModel>? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand<ContainerViewModel>(DeleteAsync);

        private async Task DeleteAsync(ContainerViewModel containerVm)
        {
            var accountId = containerVm.Database.Account.Id;
            var databaseId = containerVm.Database.Id;
            var containerId = containerVm.Id;

            if (!_dialogService.Confirm($"Are you sure you want to delete container '{containerVm.Id}'?"))
                return;

            var container = await _accountManager.GetContainerAsync(accountId, databaseId, containerId);
            await _accountManager.DeleteContainerAsync(accountId, databaseId, containerId);
            _messenger.Publish(new ContainerDeletedMessage(accountId, databaseId, container));
        }

        #endregion

        #region New query sheet

        private DelegateCommand<ContainerViewModel>? _newQuerySheetCommand;
        public ICommand NewQuerySheetCommand => _newQuerySheetCommand ??= new DelegateCommand<ContainerViewModel>(NewQuerySheet);

        private void NewQuerySheet(ContainerViewModel containerVm)
        {
            _messenger.Publish(new NewQuerySheetMessage(
                containerVm.Database.Account.Id,
                containerVm.Database.Id,
                containerVm.Id));
        }

        #endregion
    }
}
