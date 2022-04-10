using System.Windows.Input;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Helpers;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public class DatabaseEditorViewModel : DialogViewModelBase
    {
        private readonly string? _eTag;

        public DatabaseEditorViewModel(CosmosDatabase? database, int? throughput, bool isServerlessAccount)
        {
            _id = database?.Id ?? string.Empty;
            _eTag = database?.ETag;
            _throughput = isServerlessAccount ? 0 : throughput ?? 400;
            _provisionThroughput = throughput.HasValue;
            IsNew = database is null;
            CanProvisionThroughput = IsNew && !isServerlessAccount;
            IsServerlessAccount = isServerlessAccount;

            Title = IsNew
                ? "Add database"
                : "Edit database";

            _saveCommand = new DelegateCommand(Save, CanSave);
            AddOkButton(button => button.Command = _saveCommand);
            AddCancelButton();

            Validator = new ViewModelValidator<DatabaseEditorViewModel>(this);
            Validator.AddValidator(vm => vm.Id, id => CosmosHelper.ValidateId(id, "database id"));
            Validator.AddValidator(vm => vm.Throughput, throughput => CosmosHelper.ValidateThroughput(throughput, ProvisionThroughput));
        }

        public ViewModelValidator<DatabaseEditorViewModel> Validator { get; }

        private string _id;
        public string Id
        {
            get => _id;
            set => Set(ref _id, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        public bool IsNew { get; }
        public bool IsEditing => !IsNew;

        private int _throughput;
        public int Throughput
        {
            get => _throughput;
            set => Set(ref _throughput, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private bool _provisionThroughput;
        public bool ProvisionThroughput
        {
            get => _provisionThroughput;
            set => Set(ref _provisionThroughput, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        public bool IsServerlessAccount { get; }

        public bool CanProvisionThroughput { get; }

        private readonly DelegateCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private void Save()
        {
            Close(true);
        }

        private bool CanSave() => Validator?.HasError is false;

        public (CosmosDatabase database, int? throughput) GetDatabase()
        {
            var database = new CosmosDatabase
            {
                Id = Id,
                ETag = _eTag
            };
            int? throughput = ProvisionThroughput
                ? Throughput
                : default(int?);

            return (database, throughput);
        }
    }
}
