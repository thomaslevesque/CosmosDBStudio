using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace CosmosDBStudio.Avalonia.Behaviors;

public class FocusOnLoadBehavior : Behavior<InputElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.AttachedToVisualTree += OnAssociatedObjectAttachedToVisualTree;
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.AttachedToVisualTree -= OnAssociatedObjectAttachedToVisualTree;
        base.OnDetaching();
    }
    
    private void OnAssociatedObjectAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is InputElement element)
            element.Focus();
    }
}