using System;
using System.Collections.Generic;

namespace CosmosDBStudio.Dialogs
{
    public interface IDialogViewModel
    {
        string Title { get; }
        IEnumerable<DialogButton> Buttons { get; }
        bool HasButtons { get; }
        event EventHandler<bool?> CloseRequested;
        void OnClosed(bool? result);
    }

    public interface ISizableDialog
    {
        double Width { get; set; }
        double Height { get; set; }
        bool IsResizable { get; }
    }
}
