using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace CosmosDBStudio.Avalonia.View.Controls;

public class TreeNodeControl : ContentControl, IStyleable
{
    Type IStyleable.StyleKey => typeof(TreeNodeControl);

    public static readonly StyledProperty<IImage> IconProperty =
        AvaloniaProperty.Register<TreeNodeControl, IImage>("Icon");

    public IImage Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}