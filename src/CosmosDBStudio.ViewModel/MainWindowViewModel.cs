using System;
using CosmosDBStudio.Model;
using EssentialMVVM;
using Hamlet;
using Linq.Extras;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel.Commands;
using CosmosDBStudio.ViewModel.EditorTabs;
using CosmosDBStudio.ViewModel.Messages;
using CosmosDBStudio.ViewModel.Services;

namespace CosmosDBStudio.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly Lazy<IApplication> _application;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IMessenger _messenger;
        private readonly IDialogService _dialogService;
        private readonly IQueryPersistenceService _queryPersistenceService;
        private readonly AccountCommands _accountCommands;

        public MainWindowViewModel(
            Lazy<IApplication> application,
            IViewModelFactory viewModelFactory,
            IMessenger messenger,
            IDialogService dialogService,
            IQueryPersistenceService queryPersistenceService,
            AccountCommands accountCommands)
        {
            _application = application;
            _viewModelFactory = viewModelFactory;
            _messenger = messenger;
            _dialogService = dialogService;
            _queryPersistenceService = queryPersistenceService;
            _accountCommands = accountCommands;
            Tabs = new ObservableCollection<TabViewModelBase>();
            Accounts = _viewModelFactory.CreateAccounts();

            _messenger.Subscribe(this).To<NewQuerySheetMessage>((vm, message) => vm.OnNewQuerySheetMessage(message));
            _messenger.Subscribe(this).To<OpenScriptMessage<CosmosStoredProcedure>>((vm, message) => vm.OnOpenScriptMessage(message, _viewModelFactory.CreateStoredProcedureEditor));
            _messenger.Subscribe(this).To<OpenScriptMessage<CosmosUserDefinedFunction>>((vm, message) => vm.OnOpenScriptMessage(message, _viewModelFactory.CreateUserDefinedFunctionEditor));
            _messenger.Subscribe(this).To<OpenScriptMessage<CosmosTrigger>>((vm, message) => vm.OnOpenScriptMessage(message, _viewModelFactory.CreateTriggerEditor));
            _messenger.Subscribe(this).To<SetStatusBarMessage>((vm, message) => vm.OnSetStatusBarMessage(message));

            MruList = new ObservableCollection<string>(_queryPersistenceService.LoadMruList());
            LoadWorkspace();
        }

        private void OnNewQuerySheetMessage(NewQuerySheetMessage message)
        {
            var querySheet = new QuerySheet();
            var vm = _viewModelFactory.CreateQuerySheet(querySheet, null, message.Context);
            vm.CloseRequested += OnTabCloseRequested;
            Tabs.Add(vm);
            CurrentTab = vm;
        }

        private void OnOpenScriptMessage<TScript>(OpenScriptMessage<TScript> message, Func<TScript, IContainerContext, TabViewModelBase> createViewModel)
        {
            var vm = createViewModel(message.Script, message.Context);
            vm.CloseRequested += OnTabCloseRequested;
            Tabs.Add(vm);
            CurrentTab = vm;
        }

        private void OnSetStatusBarMessage(SetStatusBarMessage message)
        {
            StatusBarContent = message.Text;
        }

        private void OnTabCloseRequested(object? sender, EventArgs e)
        {
            var tab = (TabViewModelBase)sender!;
            if (!ConfirmCloseTab(tab))
                return;

            Tabs.Remove(tab);
            tab.CloseRequested -= OnTabCloseRequested;
        }

        private bool ConfirmCloseTab(TabViewModelBase tab)
        {
            if (tab is ISaveable saveable && saveable.HasChanges)
            {
                var confirmation = _dialogService.YesNoCancel($"This {tab.Description} has unsaved changes. Do you want to save before closing it?");
                if (confirmation.TryGetValue(out bool save))
                {
                    if (save)
                    {
                        // User wants to save before closing
                        // Close if they actually saved
                        return Save(saveable);
                    }
                    else
                    {
                        // User doesn't want to save, just close
                        return true;
                    }
                }

                // User cancelled, don't close
                return false;
            }

            // Nothing to save, just close
            return true;
        }

        public AccountsViewModel Accounts { get; }

        public ObservableCollection<TabViewModelBase> Tabs { get; }

        private TabViewModelBase? _currentTab;
        public TabViewModelBase? CurrentTab
        {
            get => _currentTab;
            set => Set(ref _currentTab, value);
        }

        public ObservableCollection<string> MruList { get; }

        public bool HasMru => !MruList.IsNullOrEmpty();

        public ICommand AddAccountCommand => _accountCommands.AddCommand;

        private DelegateCommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new DelegateCommand(SaveCurrentTab, CanSaveCurrentTab);

        private DelegateCommand? _saveAsCommand;
        public ICommand SaveAsCommand => _saveAsCommand ??= new DelegateCommand(SaveCurrentTabAs, CanSaveCurrentTabAs);

        private DelegateCommand<string>? _openQuerySheetCommand;
        public ICommand OpenQuerySheetCommand => _openQuerySheetCommand ??= new DelegateCommand<string>(OpenQuerySheet);

        private DelegateCommand? _quitCommand;
        public ICommand QuitCommand => _quitCommand ??= new DelegateCommand(Quit);

        private void Quit()
        {
            SaveWorkspace();
            _application.Value.Quit();
        }

        private DelegateCommand? _aboutCommand;
        public ICommand AboutCommand => _aboutCommand ??= new DelegateCommand(About);

        private void About()
        {
            var vm = new AboutViewModel(_application.Value);
            _dialogService.ShowDialog(vm);
        }

        private bool Save(ISaveable saveable)
        {
            if (string.IsNullOrEmpty(saveable.FilePath))
                return SaveAs(saveable);

            saveable.Save(saveable.FilePath);
            return true;
        }

        private bool SaveAs(ISaveable saveable)
        {
            var pathOption = _dialogService.PickFileToSave(
                filter: saveable.FileFilter,
                filterIndex: 0,
                fileName: saveable.FilePath.SomeIfNotNull());

            if (pathOption.TryGetValue(out var path))
            {
                saveable.Save(path);
                return true;
            }

            return false;
        }

        private bool CanSaveCurrentTab() => CurrentTab is ISaveable;

        private void SaveCurrentTab()
        {
            if (CurrentTab is ISaveable saveable)
                Save(saveable);
        }

        private bool CanSaveCurrentTabAs() => CurrentTab is ISaveable;

        private void SaveCurrentTabAs()
        {
            if (CurrentTab is ISaveable saveable)
                SaveAs(saveable);
        }

        private void OpenQuerySheet(string path)
        {
            if (path is null)
            {
                var pathOption = _dialogService.PickFileToOpen(
                filter: QuerySheet.FileFilter,
                filterIndex: 0);

                if (!pathOption.TryGetValue(out path))
                {
                    return;
                }
            }

            var querySheet = _queryPersistenceService.Load(path);
            if (querySheet is null)
                return;

            var vm = _viewModelFactory.CreateQuerySheet(
                querySheet,
                path,
                null);

            vm.CloseRequested += OnTabCloseRequested;
            Tabs.Add(vm);
            CurrentTab = vm;

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
            foreach (var vm in Tabs.OfType<QuerySheetViewModel>())
            {
                var sheet = new WorkspaceQuerySheet
                {
                    Title = vm.Title,
                    HasChanges = vm.HasChanges,
                    SavedPath = vm.FilePath,
                    IsCurrent = vm == CurrentTab
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
                if (querySheet is null)
                    continue;
                var vm = _viewModelFactory.CreateQuerySheet(
                    querySheet,
                    sheet.SavedPath,
                    null);

                vm.SetHasChanges(sheet.HasChanges);
                vm.CloseRequested += OnTabCloseRequested;
                Tabs.Add(vm);

                if (sheet.IsCurrent)
                    currentVM = vm;
            }

            QuerySheetViewModel.UntitledCounter = workspace.UntitledCounter;
            CurrentTab = currentVM ?? Tabs.FirstOrDefault();
        }
    }
}
