using EssentialMVVM;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class CommandViewModel : BindableBase
    {
        public CommandViewModel(string text, ICommand command, object? commandParameter = null)
        {
            _text = text;
            _command = command;
            _commandParameter = commandParameter;
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        private ICommand _command;
        public ICommand Command
        {
            get => _command;
            set => Set(ref _command, value);
        }

        private object? _commandParameter;
        public object? CommandParameter
        {
            get => _commandParameter;
            set => Set(ref _commandParameter, value);
        }
    }
}
