using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class ContainerPickerViewModel : DialogViewModelBase
    {
        private readonly IAccountContextFactory _accountContextFactory;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IAccountDirectory _accountDirectory;

        public ContainerPickerViewModel(
            IAccountContextFactory accountContextFactory,
            IViewModelFactory viewModelFactory,
            IAccountDirectory accountDirectory)
        {
            _accountContextFactory = accountContextFactory;
            _viewModelFactory = viewModelFactory;
            _accountDirectory = accountDirectory;
            AddCancelButton();
            AddOkButton(button =>
            {
                button.Text = "Select";
                button.Command = SelectCommand;
            });

            RootNodes = new ObservableCollection<TreeNodeViewModel>();
            Title = "Pick a container";
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            var nodes = _accountDirectory.GetRootNodes();
            foreach (var node in nodes)
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
                .AndNotifyPropertyChanged(nameof(SelectedContainer))
                .AndRaiseCanExecuteChanged(_selectCommand);
        }

        public ContainerNodeViewModel? SelectedContainer => SelectedItem as ContainerNodeViewModel;

        private DelegateCommand? _selectCommand;
        public ICommand SelectCommand => _selectCommand ??= new DelegateCommand(Select, CanSelect);

        private void Select() => Close(true);

        private bool CanSelect() => !(SelectedContainer is null);
    }
}
