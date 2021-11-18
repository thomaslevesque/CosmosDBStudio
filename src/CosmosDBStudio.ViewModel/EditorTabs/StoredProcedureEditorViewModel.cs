using CosmosDBStudio.Extensions;
using CosmosDBStudio.Helpers;
using CosmosDBStudio.Messages;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using CosmosDBStudio.ViewModel.EditorTabs;
using EssentialMVVM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class StoredProcedureEditorViewModel : ScriptEditorViewModelBase<CosmosStoredProcedure>
    {
        public StoredProcedureEditorViewModel(
            CosmosStoredProcedure script,
            IContainerContext containerContext,
            IMessenger messenger)
            : base(script, containerContext)
        {
            _messenger = messenger;
            Parameters = new StoredProcedureParametersViewModel();
            Parameters.AddPlaceholder();
        }

        public StoredProcedureParametersViewModel Parameters { get; }

        public override string Description => "stored procedure";

        protected override Task CreateScriptAsync(CosmosStoredProcedure script) =>
            ContainerContext.Scripts.CreateStoredProcedureAsync(script, default);

        protected override Task ReplaceScriptAsync(CosmosStoredProcedure script) =>
            ContainerContext.Scripts.ReplaceStoredProcedureAsync(script, default);

        private bool _showExecutionPanel;
        public bool ShowExecutionPanel
        {
            get => _showExecutionPanel;
            set => Set(ref _showExecutionPanel, value);
        }

        private AsyncDelegateCommand? _executeCommand;
        private readonly IMessenger _messenger;

        public ICommand ExecuteCommand => _executeCommand ??= new AsyncDelegateCommand(ExecuteAsync, CanExecute);

        private async Task ExecuteAsync()
        {
            if (!JsonHelper.TryParseJsonValue(Parameters.PartitionKeyRawValue, out object? partitionKey))
                return;

            Parameters.PartitionKeyMRU.PushMRU(Parameters.PartitionKeyRawValue!, 10);

            var parameters = new List<object?>();
            foreach (var p in Parameters.Parameters)
            {
                if (p.IsPlaceholder)
                    continue;
                if (p.Errors.HasError)
                    return;

                if (p.TryParseValue(out var value))
                {
                    parameters.Add(value);
                    p.MRU.PushMRU(p.RawValue!, 10);
                }
            }

            StoredProcedureResult? result;
            try
            {
                IsExecuting = true;
                _messenger.Publish(new SetStatusBarMessage($"Executing stored procedure '{Script.Id}'..."));
                result = await ContainerContext.Scripts.ExecuteStoredProcedureAsync(
                    Script,
                    partitionKey,
                    parameters.ToArray(),
                    default);
                _messenger.Publish(new SetStatusBarMessage($"Stored procedure executed in {result.TimeElapsed}. Status code: {result.StatusCode}. Request charge: {result.RequestCharge} RU/s"));
            }
            finally
            {
                IsExecuting = false;
            }

            Result = new StoredProcedureResultViewModel(result);
        }

        private bool CanExecute() => !Parameters.Parameters.Any(p => p.Errors.HasError);

        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            set => Set(ref _isExecuting, value);
        }

        private StoredProcedureResultViewModel? _result;
        public StoredProcedureResultViewModel? Result
        {
            get => _result;
            set => Set(ref _result, value)
                .AndNotifyPropertyChanged(nameof(HasResult));
        }

        public bool HasResult => Result != null;
    }

    public class StoredProcedureParametersViewModel : ParametersViewModel<StoredProcedureParameterViewModel>
    {
        public StoredProcedureParametersViewModel()
        {
            Errors = new ViewModelValidator<StoredProcedureParametersViewModel>(this);
            Errors.AddValidator(vm => vm.PartitionKeyRawValue, ValidatePartitionKey);
        }

        private static string ValidatePartitionKey(string? rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
                return "Partition key must be specified";

            if (!JsonHelper.TryParseJsonValue(rawValue, out _))
                return "Invalid JSON value";

            return string.Empty;
        }

        public ViewModelValidator<StoredProcedureParametersViewModel> Errors { get; }

        private string _partitionKeyRawValue = string.Empty;
        public string? PartitionKeyRawValue
        {
            get => _partitionKeyRawValue;
            set => Set(ref _partitionKeyRawValue, value)
                .AndExecute(Errors.Refresh);
        }

        private ObservableCollection<string> _partitionKeyMRU = new ObservableCollection<string>();
        public ObservableCollection<string> PartitionKeyMRU
        {
            get => _partitionKeyMRU;
            set => Set(ref _partitionKeyMRU, value);
        }
    }

    public class StoredProcedureParameterViewModel : ParameterViewModelBase<StoredProcedureParameterViewModel>
    {
    }
}
