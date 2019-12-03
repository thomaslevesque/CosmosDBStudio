using CosmosDBStudio.Dialogs;

namespace CosmosDBStudio.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(IDialogViewModel dialog);
        bool Confirm(string text);
    }
}
