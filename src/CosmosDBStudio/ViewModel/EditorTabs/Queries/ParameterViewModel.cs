using EssentialMVVM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class ParameterViewModel : BindableBase
    {
        public ParameterViewModel()
        {
            MRU = new ObservableCollection<string>();
            Errors = new ViewModelValidator<ParameterViewModel>(this);
            Errors.AddValidator(
                vm => vm.Name,
                ValidateName);
            Errors.AddValidator(
                vm => vm.RawValue,
                ValidateValue);
        }

        private string? ValidateValue(string? rawValue)
        {
            if (IsPlaceholder)
                return null;

            if (!TryParseParameterValue(rawValue, out _))
                return "Invalid value";

            return null;
        }

        private string? ValidateName(string? name)
        {
            if (IsPlaceholder)
                return null;

            if (string.IsNullOrEmpty(name))
                return "Name must be set";

            if (!Regex.IsMatch(name, @"^@?[a-zA-Z_][a-zA-Z0-9_]*$"))
                return "Invalid characters in name";

            return null;
        }

        public ViewModelValidator<ParameterViewModel> Errors { get; }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => Set(ref _name, value)
                .AndExecute(() => NameOrValueChanged());
        }

        private string? _rawValue;
        public string? RawValue
        {
            get => _rawValue;
            set => Set(ref _rawValue, value)
                .AndExecute(() => NameOrValueChanged());
        }

        public ObservableCollection<string> MRU { get; }

        private bool _isPlaceholder;
        public bool IsPlaceholder
        {
            get => _isPlaceholder;
            set => Set(ref _isPlaceholder, value);
        }

        private void NameOrValueChanged()
        {
            if (IsPlaceholder && (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(RawValue)))
            {
                IsPlaceholder = false;
                Created?.Invoke(this, EventArgs.Empty);
            }

            Errors?.Refresh();
            _deleteCommand?.RaiseCanExecuteChanged();
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

        public bool TryParseParameterValue(string? rawValue, out object? value)
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
