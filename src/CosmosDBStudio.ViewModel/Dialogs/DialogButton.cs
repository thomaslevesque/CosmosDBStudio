using System.Windows.Input;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public class DialogButton
    {
        public string Text { get; set; } = string.Empty;
        public bool? DialogResult { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCancel { get; set; }
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }
    }
}
