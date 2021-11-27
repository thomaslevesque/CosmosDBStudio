param (
    [Parameter(Mandatory=$true)]
    [string]
    $Name
)

$xaml = @"
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="CosmosDBStudio.Avalonia.View.$Name">
  
</UserControl>
"@

$cs = @"
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.View;

public partial class $Name : UserControl
{
    public $Name()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
"@

$xaml | Out-File -FilePath "$Name.axaml" -Encoding utf8
$cs | Out-File -FilePath "$Name.axaml.cs" -Encoding utf8
