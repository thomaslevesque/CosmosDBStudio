using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;

namespace CosmosDBStudio.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IContainerContextFactory _containerContextFactory;
        private readonly IMessenger _messenger;
        private readonly IDialogService _dialogService;
        private readonly IQueryPersistenceService _queryPersistenceService;

        public MainWindowViewModel(
            IViewModelFactory viewModelFactory,
            IContainerContextFactory containerContextFactory,
            IMessenger messenger,
            IDialogService dialogService,
            IQueryPersistenceService queryPersistenceService)
        {
            _viewModelFactory = viewModelFactory;
            _containerContextFactory = containerContextFactory;
            _messenger = messenger;
            _dialogService = dialogService;
            _queryPersistenceService = queryPersistenceService;
            QuerySheets = new ObservableCollection<QuerySheetViewModel>();
            Accounts = _viewModelFactory.CreateAccountsViewModel();

            _messenger.Subscribe(this).To<NewQuerySheetMessage>((vm, message) => vm.OnNewQuerySheetMessage(message));

            //AddDummyQuerySheet();
        }

        private async void OnNewQuerySheetMessage(NewQuerySheetMessage message)
        {
            try
            {
                var querySheet = new QuerySheet
                {
                    AccountId = message.AccountId,
                    DatabaseId = message.DatabaseId,
                    ContainerId = message.ContainerId
                };

                var context = await _containerContextFactory.CreateAsync(
                    message.AccountId,
                    message.DatabaseId,
                    message.ContainerId,
                    default);
                var vm = _viewModelFactory.CreateQuerySheetViewModel(context, querySheet, null);
                vm.CloseRequested += OnQuerySheetCloseRequested;
                QuerySheets.Add(vm);
                CurrentQuerySheet = vm;
            }
            catch
            {
                // TODO show error
            }
        }

        private void OnQuerySheetCloseRequested(object? sender, EventArgs e)
        {
            var sheet = (QuerySheetViewModel)sender!;
            QuerySheets.Remove(sheet);
            sheet.CloseRequested -= OnQuerySheetCloseRequested;
        }

        public AccountsViewModel Accounts { get; }

        public ObservableCollection<QuerySheetViewModel> QuerySheets { get; }

        private QuerySheetViewModel? _currentQuerySheet;

        public QuerySheetViewModel? CurrentQuerySheet
        {
            get => _currentQuerySheet;
            set => Set(ref _currentQuerySheet, value);
        }

        private AsyncDelegateCommand? _saveQuerySheetCommand;
        public ICommand SaveQuerySheetCommand => _saveQuerySheetCommand ??= new AsyncDelegateCommand(SaveCurrentQuerySheetAsync);

        private AsyncDelegateCommand? _saveQuerySheetAsCommand;
        public ICommand SaveQuerySheetAsCommand => _saveQuerySheetAsCommand ??= new AsyncDelegateCommand(SaveCurrentQuerySheetAsAsync);

        private AsyncDelegateCommand? _openQuerySheetCommand;
        public ICommand OpenQuerySheetCommand => _openQuerySheetCommand ??= new AsyncDelegateCommand(OpenQuerySheetAsync);

        public DelegateCommand? _quitCommand;
        public ICommand QuitCommand => _quitCommand ??= new DelegateCommand(Quit);

        private void Quit()
        {
            // TODO: abstraction
            App.Current.Shutdown();
        }

        private const string QueryFileFilter = "Cosmos DB Studio query sheet|*.cdbsqs";

        private async Task SaveCurrentQuerySheetAsync()
        {
            if (CurrentQuerySheet is QuerySheetViewModel vm)
            {
                if (string.IsNullOrEmpty(vm.FilePath))
                {
                    await SaveQuerySheetAsAsync(vm);
                }
                else
                {
                    var querySheet = vm.GetQuerySheet();
                    await _queryPersistenceService.SaveAsync(querySheet, vm.FilePath);
                }
            }
        }

        private async Task SaveCurrentQuerySheetAsAsync()
        {
            if (CurrentQuerySheet is QuerySheetViewModel vm)
            {
                await SaveQuerySheetAsAsync(vm);
            }
        }

        private async Task SaveQuerySheetAsAsync(QuerySheetViewModel vm)
        {
            var pathOption = _dialogService.PickFileToSave(
                    filter: QueryFileFilter,
                    filterIndex: 0,
                    fileName: vm.FilePath.SomeIfNotNull());

            if (pathOption.TryGetValue(out var path))
            {
                var querySheet = vm.GetQuerySheet();
                await _queryPersistenceService.SaveAsync(querySheet, path);
                vm.FilePath = path;
            }
        }

        private async Task OpenQuerySheetAsync()
        {
            var pathOption = _dialogService.PickFileToOpen(
                filter: QueryFileFilter,
                filterIndex: 0);

            if (pathOption.TryGetValue(out var path))
            {
                await OpenQuerySheetAsync(path);
            }
        }

        private async Task OpenQuerySheetAsync(string path)
        {
            var querySheet = await _queryPersistenceService.LoadAsync(path);
            var context = await _containerContextFactory.CreateAsync(
                    querySheet.AccountId,
                    querySheet.DatabaseId,
                    querySheet.ContainerId,
                    default);

            var vm = _viewModelFactory.CreateQuerySheetViewModel(
                context,
                querySheet,
                path);

            vm.CloseRequested += OnQuerySheetCloseRequested;
            QuerySheets.Add(vm);
            CurrentQuerySheet = vm;
        }
    }
}
