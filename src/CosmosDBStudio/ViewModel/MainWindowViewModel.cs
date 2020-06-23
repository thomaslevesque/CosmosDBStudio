using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;
using Linq.Extras;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
            _messenger.Subscribe(this).To<SetStatusBarMessage>((vm, message) => vm.OnSetStatusBarMessage(message));

            MruList = new ObservableCollection<string>(_queryPersistenceService.LoadMruList());
        }

        private async void OnNewQuerySheetMessage(NewQuerySheetMessage message)
        {
            try
            {
                var querySheet = new QuerySheet();
                var context = await _containerContextFactory.CreateAsync(
                    message.AccountId,
                    message.DatabaseId,
                    message.ContainerId,
                    default);
                var vm = _viewModelFactory.CreateQuerySheetViewModel(querySheet, null, context);
                vm.CloseRequested += OnQuerySheetCloseRequested;
                QuerySheets.Add(vm);
                CurrentQuerySheet = vm;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void OnSetStatusBarMessage(SetStatusBarMessage message)
        {
            StatusBarContent = message.Text;
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

        public ObservableCollection<string> MruList { get; }

        public bool HasMru => !MruList.IsNullOrEmpty();

        private DelegateCommand? _saveQuerySheetCommand;
        public ICommand SaveQuerySheetCommand => _saveQuerySheetCommand ??= new DelegateCommand(SaveCurrentQuerySheet);

        private DelegateCommand? _saveQuerySheetAsCommand;
        public ICommand SaveQuerySheetAsCommand => _saveQuerySheetAsCommand ??= new DelegateCommand(SaveCurrentQuerySheetAs);

        private DelegateCommand<string>? _openQuerySheetCommand;
        public ICommand OpenQuerySheetCommand => _openQuerySheetCommand ??= new DelegateCommand<string>(OpenQuerySheet);

        public DelegateCommand? _quitCommand;
        public ICommand QuitCommand => _quitCommand ??= new DelegateCommand(Quit);

        private void Quit()
        {
            // TODO: abstraction
            App.Current.Shutdown();
        }

        private const string QueryFileFilter = "Cosmos DB Studio query sheet|*.cdbsqs";

        private void SaveCurrentQuerySheet()
        {
            if (CurrentQuerySheet is QuerySheetViewModel vm)
            {
                if (string.IsNullOrEmpty(vm.FilePath))
                {
                    SaveQuerySheetAs(vm);
                }
                else
                {
                    var querySheet = vm.GetQuerySheet();
                    _queryPersistenceService.Save(querySheet, vm.FilePath);
                }
            }
        }

        private void SaveCurrentQuerySheetAs()
        {
            if (CurrentQuerySheet is QuerySheetViewModel vm)
            {
                SaveQuerySheetAs(vm);
            }
        }

        private void SaveQuerySheetAs(QuerySheetViewModel vm)
        {
            var pathOption = _dialogService.PickFileToSave(
                    filter: QueryFileFilter,
                    filterIndex: 0,
                    fileName: vm.FilePath.SomeIfNotNull());

            if (pathOption.TryGetValue(out var path))
            {
                var querySheet = vm.GetQuerySheet();
                _queryPersistenceService.Save(querySheet, path);
                vm.FilePath = path;
            }
        }

        private void OpenQuerySheet(string path)
        {
            if (path is null)
            {
                var pathOption = _dialogService.PickFileToOpen(
                filter: QueryFileFilter,
                filterIndex: 0);

                if (!pathOption.TryGetValue(out path))
                {
                    return;
                }
            }

            var querySheet = _queryPersistenceService.Load(path);
            var vm = _viewModelFactory.CreateQuerySheetViewModel(
                querySheet,
                path,
                null);

            vm.CloseRequested += OnQuerySheetCloseRequested;
            QuerySheets.Add(vm);
            CurrentQuerySheet = vm;

            int index = MruList.IndexOf(path, StringComparer.InvariantCultureIgnoreCase);
            if (index >= 0)
            {
                MruList.RemoveAt(index);
            }
            MruList.Insert(0, path);
            OnPropertyChanged(nameof(HasMru));
            _queryPersistenceService.SaveMruList(MruList);
        }

        private string? _statusBarContent;
        public string? StatusBarContent
        {
            get => _statusBarContent;
            set => Set(ref _statusBarContent, value);
        }
    }
}
