using CosmosDBStudio.Commands;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using Hamlet;
using Linq.Extras;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly AccountCommands _accountCommands;

        public MainWindowViewModel(
            IViewModelFactory viewModelFactory,
            IContainerContextFactory containerContextFactory,
            IMessenger messenger,
            IDialogService dialogService,
            IQueryPersistenceService queryPersistenceService,
            AccountCommands accountCommands)
        {
            _viewModelFactory = viewModelFactory;
            _containerContextFactory = containerContextFactory;
            _messenger = messenger;
            _dialogService = dialogService;
            _queryPersistenceService = queryPersistenceService;
            _accountCommands = accountCommands;
            QuerySheets = new ObservableCollection<QuerySheetViewModel>();
            Accounts = _viewModelFactory.CreateAccountsViewModel();

            _messenger.Subscribe(this).To<NewQuerySheetMessage>((vm, message) => vm.OnNewQuerySheetMessage(message));
            _messenger.Subscribe(this).To<SetStatusBarMessage>((vm, message) => vm.OnSetStatusBarMessage(message));

            MruList = new ObservableCollection<string>(_queryPersistenceService.LoadMruList());
            LoadWorkspace();
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
            var (close, save) = sheet.HasChanges
                ? ConfirmCloseQuerySheet(sheet)
                : (true, false);

            if (!close)
                return;

            if (save)
            {
                if (!SaveQuerySheet(sheet))
                    return;
            }

            QuerySheets.Remove(sheet);
            sheet.CloseRequested -= OnQuerySheetCloseRequested;
        }

        private (bool close, bool save) ConfirmCloseQuerySheet(QuerySheetViewModel vm)
        {
            var confirmation = _dialogService.YesNoCancel("This query sheet has unsaved changes. Do you want to save before closing it?");
            if (confirmation.TryGetValue(out bool save))
            {
                return (true, save);
            }

            return (false, false);
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

        public ICommand AddAccountCommand => _accountCommands.AddCommand;

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
            SaveWorkspace();
            App.Current.Quit();
        }

        private const string QueryFileFilter = "Cosmos DB Studio query sheet|*.cdbsqs";

        private void SaveCurrentQuerySheet()
        {
            if (CurrentQuerySheet is QuerySheetViewModel vm)
            {
                SaveQuerySheet(vm);
            }
        }

        private void SaveCurrentQuerySheetAs()
        {
            if (CurrentQuerySheet is QuerySheetViewModel vm)
            {
                SaveQuerySheetAs(vm);
            }
        }

        private bool SaveQuerySheet(QuerySheetViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.FilePath))
            {
                return SaveQuerySheetAs(vm);
            }
            else
            {
                var querySheet = vm.GetQuerySheet();
                _queryPersistenceService.Save(querySheet, vm.FilePath);
                vm.HasChanges = false;
                return true;
            }
        }

        private bool SaveQuerySheetAs(QuerySheetViewModel vm)
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
                vm.HasChanges = false;
                return true;
            }

            return false;
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

        private void SaveWorkspace()
        {
            var workspace = new Workspace();
            foreach (var vm in QuerySheets)
            {
                var sheet = new WorkspaceQuerySheet
                {
                    Title = vm.Title,
                    HasChanges = vm.HasChanges,
                    SavedPath = vm.FilePath,
                    IsCurrent = vm == CurrentQuerySheet
                };

                if (vm.HasChanges || string.IsNullOrEmpty(vm.FilePath))
                {
                    sheet.TempPath = _queryPersistenceService.SaveWorkspaceTempQuery(vm.GetQuerySheet());
                }

                workspace.QuerySheets.Add(sheet);
            }
            workspace.UntitledCounter = QuerySheetViewModel.UntitledCounter;
            _queryPersistenceService.SaveWorkspace(workspace);
        }

        private void LoadWorkspace()
        {
            QuerySheetViewModel? currentVM = null;
            var workspace = _queryPersistenceService.LoadWorkspace();
            foreach (var sheet in workspace.QuerySheets)
            {
                var path = sheet.TempPath ?? sheet.SavedPath;
                if (path is null)
                {
                    Debug.Fail("Either TempPath or SavedPath should be non-null");
                    continue;
                }

                var querySheet = _queryPersistenceService.Load(path);
                var vm = _viewModelFactory.CreateQuerySheetViewModel(
                    querySheet,
                    sheet.SavedPath,
                    null);

                vm.HasChanges = sheet.HasChanges;
                vm.CloseRequested += OnQuerySheetCloseRequested;
                QuerySheets.Add(vm);

                if (sheet.IsCurrent)
                    currentVM = vm;
            }

            QuerySheetViewModel.UntitledCounter = workspace.UntitledCounter;
            CurrentQuerySheet = currentVM;
        }
    }
}
