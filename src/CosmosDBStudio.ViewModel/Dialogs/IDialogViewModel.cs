using System;
using System.Collections.Generic;

namespace CosmosDBStudio.ViewModel.Dialogs
{
    public interface IDialogViewModel
    {
        string Title { get; }
        IEnumerable<DialogButton> Buttons { get; }
        bool HasButtons { get; }
        event EventHandler<bool?> CloseRequested;
        void OnClosing(DialogClosingEventArgs args);
        void OnClosed(bool? result);
    }

    public interface ISizableDialog
    {
        double Width { get; set; }
        double Height { get; set; }
        bool IsResizable { get; }
    }
}
