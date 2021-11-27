using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.View;

public partial class ContainerPickerView : UserControl
{
    public ContainerPickerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
