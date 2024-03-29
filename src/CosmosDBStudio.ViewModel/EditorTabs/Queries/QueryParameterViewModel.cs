﻿using System.Text.RegularExpressions;

namespace CosmosDBStudio.ViewModel.EditorTabs.Queries
{
    public class QueryParameterViewModel : ParameterViewModelBase<QueryParameterViewModel>
    {
        public QueryParameterViewModel()
        {
            Errors.AddValidator(vm => vm.Name, ValidateName);
        }

        private string ValidateName(string? name)
        {
            if (IsPlaceholder)
                return string.Empty;

            if (string.IsNullOrEmpty(name))
                return "Name must be set";

            if (!Regex.IsMatch(name, @"^@?[a-zA-Z_][a-zA-Z0-9_]*$"))
                return "Invalid characters in name";

            return string.Empty;
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => Set(ref _name, value)
                .AndExecute(DataChanged);
        }

        protected override bool HasData() => base.HasData() || !string.IsNullOrEmpty(Name);
    }
}
