using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Model;
using EssentialMVVM;
using System;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class AccountEditorViewModel : DialogViewModelBase
    {
        public AccountEditorViewModel(CosmosAccount? account = null)
        {
            _name = account?.Name ?? string.Empty;
            _endpoint = account?.Endpoint ?? string.Empty;
            _key = account?.Key ?? string.Empty;

            Title = account is null
                ? "Add Cosmos DB account"
                : "Edit Cosmos DB account";

            _saveCommand = new DelegateCommand(Save, CanSave);
            AddOkButton(button => button.Command = _saveCommand);
            AddCancelButton();

            Validator = new ViewModelValidator<AccountEditorViewModel>(this);
            Validator.AddValidator(vm => vm.Name, ValidateName);
            Validator.AddValidator(vm => vm.Endpoint, ValidateEndpoint);
            Validator.AddValidator(vm => vm.Key, ValidateKey);
        }

        private static string? ValidateName(string name)
        {
            return string.IsNullOrEmpty(name)
                    ? "A name must be specified"
                    : null;
        }

        private static string? ValidateEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return "The account endpoint must be specified";

            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out _))
                return "The specified account endpoint is not a valid URI";

            return null;
        }

        private static string? ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "The account key must be specified";
            
            try
            {
                Convert.FromBase64String(key);
                return null;
            }
            catch
            {
                return "The specified account key is invalid";
            }
        }

        public ViewModelValidator<AccountEditorViewModel> Validator { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private string _endpoint;
        public string Endpoint
        {
            get => _endpoint;
            set => Set(ref _endpoint, value)
                .AndExecute(SetNameFromEndpoint)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private string _key;
        public string Key
        {
            get => _key;
            set => Set(ref _key, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private void SetNameFromEndpoint()
        {
            if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Endpoint))
            {
                if (Uri.TryCreate(Endpoint, UriKind.Absolute, out var uri))
                {
                    Name = uri.Host.Split('.')[0];
                }
            }
        }

        private readonly DelegateCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private void Save()
        {
            Close(true);
        }

        private bool CanSave() => Validator?.HasError is false;
    }
}
