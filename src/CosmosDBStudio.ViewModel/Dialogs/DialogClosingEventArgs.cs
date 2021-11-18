using System.ComponentModel;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public class DialogClosingEventArgs : CancelEventArgs
    {
        public DialogClosingEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }

        public bool? DialogResult { get; }
    }
}
