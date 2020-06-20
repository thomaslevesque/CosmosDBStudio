using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Model;
using EssentialMVVM;
using System.Linq;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class DatabaseEditorViewModel : DialogViewModelBase
    {
        public DatabaseEditorViewModel(CosmosDatabase? database)
        {
            _id = database?.Id ?? string.Empty;
            _throughput = database?.Throughput ?? 400;
            _provisionThroughput = (database?.Throughput).HasValue;
            IsNew = database is null;

            Title = IsNew
                ? "Add database"
                : "Edit database";

            _saveCommand = new DelegateCommand(Save, CanSave);
            AddOkButton(button => button.Command = _saveCommand);
            AddCancelButton();

            Validator = new ViewModelValidator<DatabaseEditorViewModel>(this);
            Validator.AddValidator(vm => vm.Id, ValidateId);
            Validator.AddValidator(vm => vm.Throughput, ValidateThroughput);
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

        private string? ValidateThroughput(int throughput)
        {
            if (ProvisionThroughput)
            {
                if (throughput < 400)
                    return "Throughput cannot be less than 400";
            }

            return null;
        }

        private static readonly char[] ForbiddenCharactersInId = @"/\#?".ToCharArray();
        private static string? ValidateId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "The database id must be specified";

            if (id.IndexOfAny(ForbiddenCharactersInId) >= 0)
            {
                return "The database id must not contain characters "
                    + string.Join(" ", ForbiddenCharactersInId.Select(c => $"'{c}'"));
            }

            if (id.Trim() != id)
            {
                return "The database id must not start or end with space";
            }

            return null;
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
