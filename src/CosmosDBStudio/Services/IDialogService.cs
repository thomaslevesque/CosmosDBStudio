using CosmosDBStudio.Dialogs;
using Hamlet;

namespace CosmosDBStudio.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(IDialogViewModel dialog);
        bool Confirm(string text);
        Option<bool> YesNoCancel(string text);
        void ShowError(string message);

        Option<string> PickFileToSave(
            Option<string> filter = default,
            Option<int> filterIndex = default,
            Option<string> fileName = default,
            Option<string> initialDirectory = default);

        Option<string> PickFileToOpen(
            Option<string> filter = default,
            Option<int> filterIndex = default,
            Option<string> fileName = default,
            Option<string> initialDirectory = default);
    }
}
