using System.ComponentModel;

namespace CosmosDBStudio.Dialogs
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
