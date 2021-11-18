using CosmosDBStudio.ViewModel.Dialogs;
using Hamlet;

namespace CosmosDBStudio.ViewModel.Services
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

        Option<string> TextPrompt(string prompt, Option<string> initialText = default);
    }
}
