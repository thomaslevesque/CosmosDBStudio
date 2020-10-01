using EssentialMVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public abstract class ParameterViewModelBase<TViewModel> : BindableBase
        where TViewModel : ParameterViewModelBase<TViewModel>
    {
        public ParameterViewModelBase()
        {
            MRU = new ObservableCollection<string>();
            Errors = new ViewModelValidator<TViewModel>((TViewModel)this);
            Errors.AddValidator(vm => vm.RawValue, ValidateValue);
        }

        private string? _rawValue;
        public string? RawValue
        {
            get => _rawValue;
            set => Set(ref _rawValue, value)
                .AndExecute(DataChanged);
        }

        public ViewModelValidator<TViewModel> Errors { get; }

        public ObservableCollection<string> MRU { get; }

        private bool _isPlaceholder;
        public bool IsPlaceholder
        {
            get => _isPlaceholder;
            set => Set(ref _isPlaceholder, value);
        }

        public event EventHandler? Created;
        public event EventHandler? DeleteRequested;

        private DelegateCommand? _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand(Delete, CanDelete);

        private void Delete()
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanDelete() => !IsPlaceholder;

        protected void DataChanged()
        {
            if (IsPlaceholder && HasData())
            {
                IsPlaceholder = false;
                Created?.Invoke(this, EventArgs.Empty);
            }

            Errors?.Refresh();
            _deleteCommand?.RaiseCanExecuteChanged();
        }

        protected virtual bool HasData() => !string.IsNullOrEmpty(RawValue);

        private string? ValidateValue(string? rawValue)
        {
            if (IsPlaceholder)
                return null;

            if (!TryParseParameterValue(rawValue, out _))
                return "Invalid JSON value";

            return null;
        }

        public bool TryParseParameterValue(out object? value) => TryParseParameterValue(RawValue, out value);

        private static bool TryParseParameterValue(string? rawValue, out object? value)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                value = null;
                return false;
            }

            try
            {
                using var tReader = new StringReader(rawValue);
                using var jReader = new JsonTextReader(tReader)
                {
                    DateParseHandling = DateParseHandling.None
                };

                var token = JToken.ReadFrom(jReader);
                value = token.ToObject<object?>();
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}
