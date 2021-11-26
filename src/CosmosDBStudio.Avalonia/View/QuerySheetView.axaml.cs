using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.View;

public partial class QuerySheetView : UserControl
{
    public QuerySheetView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}