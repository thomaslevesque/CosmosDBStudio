using EssentialMVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CosmosDBStudio.ViewModel
{
    public class ViewModelValidator<TViewModel> : BindableBase, IDataErrorInfo
    {
        private readonly Dictionary<string, Func<TViewModel, object?>> _getters;
        private readonly Dictionary<string, Func<object?, string?>> _validators;
        private readonly TViewModel _viewModel;

        public ViewModelValidator(TViewModel viewModel)
        {
            _getters = new Dictionary<string, Func<TViewModel, object?>>();
            _validators = new Dictionary<string, Func<object?, string?>>();
            _viewModel = viewModel;
        }

        public void AddValidator<TProperty>(
            Expression<Func<TViewModel, TProperty>> property,
            Func<TProperty, string?> validator)
        {
            var propertyName = GetPropertyName(property);
            var getter = property.Compile();
            _getters.Add(propertyName, vm => getter(vm));
            _validators.Add(propertyName, value => validator((TProperty)value!));
        }

        public string? this[string columnName]
        {
            get
            {
                if (_validators.TryGetValue(columnName, out var validator))
                {
                    var value = _getters[columnName](_viewModel);
                    return validator(value);
                }

                return null;
            }
        }

        public string? Error
        {
            get
            {
                StringBuilder? builder = null;
                foreach (var (propertyName, error) in GetErrors())
                {
                    builder ??= new StringBuilder();
                    if (builder.Length > 0)
                        builder.AppendLine();
                    builder.AppendLine($"{propertyName}: {error}");
                }

                return builder?.ToString();
            }
        }

        public bool HasError => GetErrors().Any();

        public IEnumerable<(string propertyName, string error)> GetErrors()
        {
            foreach (var (propertyName, validator) in _validators)
            {
                var value = _getters[propertyName](_viewModel);
                var error = validator(value);
                if (!string.IsNullOrEmpty(error))
                    yield return (propertyName, error);
            }
        }

        private static string GetPropertyName(LambdaExpression property)
        {
            if (property.Body is MemberExpression memberExpr)
                return memberExpr.Member.Name;
            throw new ArgumentException("Invalid property expression", nameof(property));
        }

        public void Refresh()
        {
            OnPropertyChanged(string.Empty);
        }
    }
}