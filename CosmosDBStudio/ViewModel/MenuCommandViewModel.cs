using System.Windows.Input;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel
{
    public class MenuCommandViewModel : BindableBase
    {
        public MenuCommandViewModel(string text, ICommand command)
        {
            _text = text;
            _command = command;
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
    }
}
