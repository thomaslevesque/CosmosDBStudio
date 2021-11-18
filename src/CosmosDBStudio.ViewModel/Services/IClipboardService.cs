namespace CosmosDBStudio.ViewModel.Services
{
    public interface IClipboardService
    {
        bool TryGetText(out string text);
    }
}
