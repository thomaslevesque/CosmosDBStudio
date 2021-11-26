using Avalonia;
using CosmosDBStudio.ViewModel.Services;

namespace CosmosDBStudio.Avalonia.Services.Implementation;

public class ClipboardService : IClipboardService
{
    public bool TryGetText(out string text)
    {
        text = Application.Current.Clipboard.GetTextAsync().GetAwaiter().GetResult();
        if (text is not null)
            return true;
        text = string.Empty;
        return false;

    }
}