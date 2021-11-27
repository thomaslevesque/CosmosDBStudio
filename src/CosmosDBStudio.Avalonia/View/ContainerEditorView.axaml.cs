using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.View;

public partial class ContainerEditorView : UserControl
{
    public ContainerEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
