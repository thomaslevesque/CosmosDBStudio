using System.Collections.ObjectModel;
using CosmosDBStudio.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class AccountsViewModel : BindableBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IAccountDirectory _accountDirectory;

        public AccountsViewModel(IViewModelFactory viewModelFactory, IAccountDirectory accountDirectory)
        {
            _viewModelFactory = viewModelFactory;
            _accountDirectory = accountDirectory;
            Accounts = new ObservableCollection<AccountViewModel>();
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            foreach (var account in _accountDirectory.Accounts)
            {
                Accounts.Add(_viewModelFactory.CreateAccountViewModel(account));
            }
        }

        public ObservableCollection<AccountViewModel> Accounts { get; }
    }
}
