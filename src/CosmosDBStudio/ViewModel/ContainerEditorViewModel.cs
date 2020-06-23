using CosmosDBStudio.Dialogs;
using CosmosDBStudio.Helpers;
using CosmosDBStudio.Model;
using EssentialMVVM;
using System;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class ContainerEditorViewModel : DialogViewModelBase
    {
        public ContainerEditorViewModel(CosmosContainer? container, bool databaseHasProvisionedThroughput)
        {
            _id = container?.Id ?? string.Empty;
            _throughput = container?.Throughput ?? 400;
            _provisionThroughput = (container?.Throughput).HasValue;
            _partitionKeyPath = container?.PartitionKeyPath ?? string.Empty;
            _largePartitionKey = container?.LargePartitionKey ?? false;
            if (container?.DefaultTTL is int defaultTTL)
            {
                _enableTTL = true;
                _hasDefaultTTL = defaultTTL >= 0;
                _defaultTTL = Math.Max(defaultTTL, 1);
            }
            else
            {
                _enableTTL = false;
                _hasDefaultTTL = false;
                _defaultTTL = 1;
            }

            IsNew = container is null;

            if (databaseHasProvisionedThroughput)
            {
                CanChangeProvisionThroughput = IsNew;
            }
            else
            {
                _provisionThroughput = true;
                CanChangeProvisionThroughput = false;
            }

            Title = IsNew
                ? "Add container"
                : "Edit container";

            _saveCommand = new DelegateCommand(Save, CanSave);
            AddOkButton(button => button.Command = _saveCommand);
            AddCancelButton();

            Validator = new ViewModelValidator<ContainerEditorViewModel>(this);
            Validator.AddValidator(vm => vm.Id, id => CosmosHelper.ValidateId(id, "container id"));
            Validator.AddValidator(vm => vm.Throughput, throughput => CosmosHelper.ValidateThroughput(throughput, ProvisionThroughput));
            Validator.AddValidator(vm => vm.DefaultTTL, defaultTTL => CosmosHelper.ValidateDefaultTTL(defaultTTL, EnableTTL && HasDefaultTTL));
            Validator.AddValidator(vm => vm.PartitionKeyPath, CosmosHelper.ValidatePartitionKeyPath);
        }

        public ViewModelValidator<ContainerEditorViewModel> Validator { get; }

        public bool IsNew { get; }
        public bool IsEditing => !IsNew;

        private string _id;
        public string Id
        {
            get => _id;
            set => Set(ref _id, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private string _partitionKeyPath;
        public string PartitionKeyPath
        {
            get => _partitionKeyPath;
            set => Set(ref _partitionKeyPath, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private bool _largePartitionKey;
        public bool LargePartitionKey
        {
            get => _largePartitionKey;
            set => Set(ref _largePartitionKey, value);
        }

        private bool _enableTTL;
        public bool EnableTTL
        {
            get => _enableTTL;
            set => Set(ref _enableTTL, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private bool _hasDefaultTTL;
        public bool HasDefaultTTL
        {
            get => _hasDefaultTTL;
            set => Set(ref _hasDefaultTTL, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

        private int _defaultTTL;
        public int DefaultTTL
        {
            get => _defaultTTL;
            set => Set(ref _defaultTTL, value)
                .AndExecute(() => Validator?.Refresh())
                .AndRaiseCanExecuteChanged(_saveCommand);
        }

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

        public bool CanChangeProvisionThroughput { get; }

        private readonly DelegateCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private void Save()
        {
            Close(true);
        }

        private bool CanSave() => Validator?.HasError is false;

        public CosmosContainer GetContainer()
        {
            return new CosmosContainer
            {
                Id = Id,
                PartitionKeyPath = PartitionKeyPath,
                LargePartitionKey = LargePartitionKey,
                Throughput = ProvisionThroughput
                    ? Throughput
                    : default(int?),
                DefaultTTL = EnableTTL
                    ? DefaultTTL > 0
                        ? DefaultTTL
                        : -1
                    : default(int?)
            };
        }
    }
}
