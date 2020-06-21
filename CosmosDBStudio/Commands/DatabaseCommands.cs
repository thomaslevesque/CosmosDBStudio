using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using CosmosDBStudio.ViewModel;
using EssentialMVVM;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.Commands
{
    public class DatabaseCommands
    {
        public DatabaseCommands(
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

        private AsyncDelegateCommand<AccountViewModel>? _createCommand;
        private readonly Lazy<IViewModelFactory> _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly ICosmosAccountManager _accountManager;
        private readonly IMessenger _messenger;

        public ICommand CreateCommand => _createCommand ??= new AsyncDelegateCommand<AccountViewModel>(CreateAsync);

        private async Task CreateAsync(AccountViewModel accountVm)
        {
            var dialog = _viewModelFactory.Value.CreateDatabaseEditorViewModel();
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var database = new CosmosDatabase
                {
                    Id = dialog.Id,
                    Throughput = dialog.ProvisionThroughput
                        ? dialog.Throughput
                        : default(int?)
                };

                await _accountManager.CreateDatabaseAsync(accountVm.Id, database);
                _messenger.Publish(new DatabaseCreatedMessage(accountVm.Id, database));
            }
        }

        #endregion

        #region Edit

        private AsyncDelegateCommand<DatabaseViewModel>? _editCommand;
        public ICommand EditCommand => _editCommand ??= new AsyncDelegateCommand<DatabaseViewModel>(EditAsync);

        private async Task EditAsync(DatabaseViewModel databaseVm)
        {
            var database = await _accountManager.GetDatabaseAsync(databaseVm.Account.Id, databaseVm.Id);
            var dialog = _viewModelFactory.Value.CreateDatabaseEditorViewModel(database);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                database = dialog.GetDatabase();
                await _accountManager.UpdateDatabaseAsync(databaseVm.Account.Id, database);
            }
        }

        #endregion

        #region Delete

        private AsyncDelegateCommand<DatabaseViewModel>? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand<DatabaseViewModel>(DeleteAsync);

        private async Task DeleteAsync(DatabaseViewModel databaseVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete database '{databaseVm.Id}'?"))
                return;

            var accountVm = databaseVm.Account;
            var database = await _accountManager.GetDatabaseAsync(accountVm.Id, databaseVm.Id);
            await _accountManager.DeleteDatabaseAsync(accountVm.Id, databaseVm.Id);
            _messenger.Publish(new DatabaseDeletedMessage(accountVm.Id, database));
        }

        #endregion
    }
}
