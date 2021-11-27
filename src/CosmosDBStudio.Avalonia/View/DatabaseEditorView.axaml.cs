using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.View;

public partial class DatabaseEditorView : UserControl
{
    public DatabaseEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
