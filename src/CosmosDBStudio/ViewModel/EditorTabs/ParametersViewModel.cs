using EssentialMVVM;
using System;
using System.Collections.ObjectModel;

namespace CosmosDBStudio.ViewModel.EditorTabs
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
        }

        public void AddPlaceholder()
        {
            var placeholder = new T { IsPlaceholder = true };
            placeholder.Created += OnParameterCreated;
            Parameters.Add(placeholder);
        }

        private void OnParameterCreated(object? sender, EventArgs _)
        {
            if (sender is T placeholder)
            {
                placeholder.Created -= OnParameterCreated;
                placeholder.DeleteRequested += OnParameterDeleteRequested;
                AddPlaceholder();
            }
        }

        private void OnParameterDeleteRequested(object? sender, EventArgs e)
        {
            if (sender is T parameter)
            {
                Parameters.Remove(parameter);
            }
        }
    }
}
