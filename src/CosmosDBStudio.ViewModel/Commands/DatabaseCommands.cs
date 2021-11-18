using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Messages;
using CosmosDBStudio.ViewModel.Services;
using CosmosDBStudio.ViewModel.TreeNodes;
using EssentialMVVM;
using Microsoft.Azure.Cosmos;

namespace CosmosDBStudio.ViewModel.Commands
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
            var dialog = _viewModelFactory.Value.CreateDatabaseEditor(null, null, accountVm.Context.IsServerless);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var (database, throughput) = dialog.GetDatabase();
                try
                {
                    await accountVm.Context.Databases.CreateDatabaseAsync(database, throughput, default);
                    _messenger.Publish(new DatabaseCreatedMessage(accountVm.Context, database));
                }
                catch(CosmosException ex) when (throughput.HasValue && ex.StatusCode == HttpStatusCode.BadRequest && ex.Message.Contains("serverless", StringComparison.OrdinalIgnoreCase))
                {
                    _dialogService.ShowError("Failed to create database with provisioned throughput, because the account seems to be serverless. If this is a serverless account, please specify it in the account configuration.");
                }
            }
        }

        #endregion

        #region Edit

        private AsyncDelegateCommand<DatabaseNodeViewModel>? _editCommand;
        public ICommand EditCommand => _editCommand ??= new AsyncDelegateCommand<DatabaseNodeViewModel>(EditAsync);

        private async Task EditAsync(DatabaseNodeViewModel databaseVm)
        {
            var context = databaseVm.Context;
            var database = await context.GetDatabaseAsync(default);
            int? throughput;
            try
            {
                throughput = await context.GetThroughputAsync(default);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.BadRequest && ex.Message.Contains("serverless", StringComparison.OrdinalIgnoreCase))
            {
                _dialogService.ShowError("Failed to read database throughput, because the account seems to be serverless. If this is a serverless account, please specify it in the account configuration.");
                return;
            }
            var dialog = _viewModelFactory.Value.CreateDatabaseEditor(database, throughput, context.AccountContext.IsServerless);
            if (_dialogService.ShowDialog(dialog) is true)
            {
                var (_, newThroughput) = dialog.GetDatabase();
                if (newThroughput != throughput)
                    await context.SetThroughputAsync(newThroughput, default);
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
            var database = await context.GetDatabaseAsync(default);
            await context.AccountContext.Databases.DeleteDatabaseAsync(database, default);
            _messenger.Publish(new DatabaseDeletedMessage(context.AccountContext, database));
        }

        #endregion
    }
}
