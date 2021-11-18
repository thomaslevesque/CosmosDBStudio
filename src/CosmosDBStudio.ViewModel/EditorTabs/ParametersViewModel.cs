using EssentialMVVM;
using System;
using System.Collections.ObjectModel;

namespace CosmosDBStudio.ViewModel
{
    public class ParametersViewModel<T> : BindableBase
        where T : ParameterViewModelBase<T>, new()
    {
        public ParametersViewModel()
        {
            Parameters = new ObservableCollection<T>();
        }

        public ObservableCollection<T> Parameters { get; }

        public void AddParameter(T parameter)
        {
            parameter.DeleteRequested += OnParameterDeleteRequested;
            Parameters.Add(parameter);
            if (parameter.IsPlaceholder)
            {
                parameter.Created += OnParameterCreated;
            }
            else
            {
                parameter.DeleteRequested += OnParameterDeleteRequested;
                parameter.Changed += OnParameterChanged;
            }
        }

        public void AddPlaceholder()
        {
            var placeholder = new T { IsPlaceholder = true };
            placeholder.Created += OnParameterCreated;
            Parameters.Add(placeholder);
        }

        public event EventHandler? Changed;

        private void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void OnParameterChanged(object? sender, EventArgs e)
        {
            OnChanged();
        }

        private void OnParameterCreated(object? sender, EventArgs _)
        {
            if (sender is T parameter)
            {
                parameter.Created -= OnParameterCreated;
                parameter.DeleteRequested += OnParameterDeleteRequested;
                parameter.Changed += OnParameterChanged;
                AddPlaceholder();
                OnChanged();
            }
        }

        private void OnParameterDeleteRequested(object? sender, EventArgs e)
        {
            if (sender is T parameter)
            {
                parameter.DeleteRequested -= OnParameterDeleteRequested;
                parameter.Changed -= OnParameterChanged;
                Parameters.Remove(parameter);
                OnChanged();
            }
        }
    }
}
