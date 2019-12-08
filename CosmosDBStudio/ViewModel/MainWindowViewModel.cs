using System;
using System.Collections.ObjectModel;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IContainerContextFactory _containerContextFactory;
        private readonly IMessenger _messenger;

        public MainWindowViewModel(
            IViewModelFactory viewModelFactory,
            IContainerContextFactory containerContextFactory,
            IMessenger messenger)
        {
            _viewModelFactory = viewModelFactory;
            _containerContextFactory = containerContextFactory;
            _messenger = messenger;
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
                    ContainerId = message.ContainerId,
                    DefaultOptions = new QueryOptions
                    {
                        PartitionKey = null
                    }
                };

                var context = await _containerContextFactory.CreateAsync(
                    message.AccountId,
                    message.DatabaseId,
                    message.ContainerId,
                    default);
                var vm = _viewModelFactory.CreateQuerySheetViewModel(context, querySheet);
                vm.CloseRequested += CloseHandler;
                QuerySheets.Add(vm);
                CurrentQuerySheet = vm;
            }
            catch
            {
                // TODO show error
            }

            void CloseHandler(object? sender, EventArgs e)
            {
                var sheet = (QuerySheetViewModel)sender!;
                QuerySheets.Remove(sheet);
                sheet.CloseRequested -= CloseHandler;
            }
        }

        public AccountsViewModel Accounts { get; }

        public ObservableCollection<QuerySheetViewModel> QuerySheets { get; }

        private QuerySheetViewModel? _currentQuerySheet;

        public QuerySheetViewModel? CurrentQuerySheet
        {
            get => _currentQuerySheet;
            set => Set(ref _currentQuerySheet, value);
        }
    }
}
