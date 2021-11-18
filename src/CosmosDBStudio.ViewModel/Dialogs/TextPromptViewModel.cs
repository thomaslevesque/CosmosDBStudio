using System.Windows.Input;
using EssentialMVVM;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public class TextPromptViewModel : DialogViewModelBase
    {
        public TextPromptViewModel(string prompt, string? text)
        {
            _prompt = prompt;
            _text = text ?? string.Empty;
            AddOkButton(button => button.Command = SubmitCommand);
            AddCancelButton();
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value)
                .AndRaiseCanExecuteChanged(_submitCommand);
        }

        private string _prompt;
        public string Prompt
        {
            get => _prompt;
            set => Set(ref _prompt, value);
        }

        private DelegateCommand? _submitCommand;
        public ICommand SubmitCommand => _submitCommand ??= new DelegateCommand(Submit, CanSubmit);

        private void Submit() => Close(true);

        private bool CanSubmit() => !string.IsNullOrEmpty(Text);
    }
}
