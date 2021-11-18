using System;
using System.Data.Common;
using System.Windows.Input;
using CosmosDBStudio.Model;
using CosmosDBStudio.ViewModel.Services;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public class AccountEditorViewModel : DialogViewModelBase
    {
        public AccountEditorViewModel(CosmosAccount? account, IClipboardService clipboardService)
        {
            _name = account?.Name ?? string.Empty;
            _endpoint = account?.Endpoint ?? string.Empty;
            _key = account?.Key ?? string.Empty;
            _isServerless = account?.IsServerless ?? false;
            _folder = account?.Folder ?? string.Empty;

            if (account is null && clipboardService.TryGetText(out string copiedText))
            {
                TrySetValuesFromConnectionString(copiedText);
            }

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

        private void TrySetValuesFromConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            var builder = new DbConnectionStringBuilder();

            try
            {
                builder.ConnectionString = connectionString;
            }
            catch (ArgumentException)
            {
                return;
            }

            if (builder.TryGetValue("AccountEndpoint", out var tmp) && tmp is string endpoint)
                _endpoint = endpoint;
            if (builder.TryGetValue("AccountKey", out tmp) && tmp is string key)
                _key = key;
            SetNameFromEndpoint();
        }

        private static string ValidateName(string name)
        {
            return string.IsNullOrEmpty(name)
                    ? "A name must be specified"
                    : string.Empty;
        }

        private static string ValidateEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return "The account endpoint must be specified";

            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out _))
                return "The specified account endpoint is not a valid URI";

            return string.Empty;
        }

        private static string ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "The account key must be specified";

            try
            {
                Convert.FromBase64String(key);
                return string.Empty;
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

        private bool _isServerless;
        public bool IsServerless
        {
            get => _isServerless;
            set => Set(ref _isServerless, value);
        }


        private string _folder;
        public string Folder
        {
            get => _folder;
            set => Set(ref _folder, value);
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
