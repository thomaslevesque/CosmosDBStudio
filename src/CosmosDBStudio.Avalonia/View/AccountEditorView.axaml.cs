using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.View;

public partial class AccountEditorView : UserControl
{
    public AccountEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
