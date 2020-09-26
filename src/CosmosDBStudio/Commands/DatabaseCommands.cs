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
        private readonly Lazy<IViewModelFactory> _viewModelFactory;
        private readonly IDialogService _dialogService;
        private readonly IMessenger _messenger;

        public DatabaseCommands(
            Lazy<IViewModelFactory> viewModelFactory,
            IDialogService dialogService,
            IMessenger messenger)
        {
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _messenger = messenger;
        }

        #region Create

        private AsyncDelegateCommand<AccountNodeViewModel>? _createCommand;
        public ICommand CreateCommand => _createCommand ??= new AsyncDelegateCommand<AccountNodeViewModel>(CreateAsync);

        private async Task CreateAsync(AccountNodeViewModel accountVm)
        {
            var dialog = _viewModelFactory.Value.CreateDatabaseEditor();
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var database = new CosmosDatabase
                {
                    Id = dialog.Id,
                    Throughput = dialog.ProvisionThroughput
                        ? dialog.Throughput
                        : default(int?)
                };

                await accountVm.Context.Databases.CreateDatabaseAsync(database, default);
                _messenger.Publish(new DatabaseCreatedMessage(accountVm.Context, database));
            }
        }

        #endregion

        #region Edit

        private AsyncDelegateCommand<DatabaseNodeViewModel>? _editCommand;
        public ICommand EditCommand => _editCommand ??= new AsyncDelegateCommand<DatabaseNodeViewModel>(EditAsync);

        private async Task EditAsync(DatabaseNodeViewModel databaseVm)
        {
            var context = databaseVm.Context;
            var database = await context.AccountContext.Databases.GetDatabaseAsync(context.DatabaseId, default);
            var dialog = _viewModelFactory.Value.CreateDatabaseEditor(database);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                database = dialog.GetDatabase();
                await context.AccountContext.Databases.UpdateDatabaseAsync(database, default);
            }
        }

        #endregion

        #region Delete

        private AsyncDelegateCommand<DatabaseNodeViewModel>? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new AsyncDelegateCommand<DatabaseNodeViewModel>(DeleteAsync);

        private async Task DeleteAsync(DatabaseNodeViewModel databaseVm)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete database '{databaseVm.Id}'?"))
                return;

            var context = databaseVm.Context;
            var database = await context.AccountContext.Databases.GetDatabaseAsync(context.DatabaseId, default);
            await context.AccountContext.Databases.DeleteDatabaseAsync(database, default);
            _messenger.Publish(new DatabaseDeletedMessage(context.AccountContext, database));
        }

        #endregion
    }
}
