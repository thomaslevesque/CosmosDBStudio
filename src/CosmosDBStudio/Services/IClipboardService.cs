namespace CosmosDBStudio.Services
{
    public interface IClipboardService
    {
        bool TryGetText(out string text);
    }
}
