using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace CosmosDBStudio.Avalonia.Behaviors;

public class ErrorBehavior : Behavior<Control>
{
    static ErrorBehavior()
    {
        ErrorProperty.Changed.Subscribe(onNext: OnErrorPropertyChanged);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.AttachedToVisualTree += OnAssociatedObjectAttachedToVisualTree;
        AssociatedObject!.PropertyChanged += OnAssociatedObjectPropertyChanged;
        if (Error is string)
        {
            AddErrorAdorner();
        }
    }

    private void OnAssociatedObjectPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.IsVisibleProperty)
            OnErrorChanged(Error);
    }

    private void OnAssociatedObjectAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        OnErrorChanged(Error);
    }

    protected override void OnDetaching()
    {
        RemoveErrorAdorner();
        AssociatedObject!.PropertyChanged -= OnAssociatedObjectPropertyChanged;
        AssociatedObject!.AttachedToVisualTree -= OnAssociatedObjectAttachedToVisualTree;
        base.OnDetaching();
    }

    public string? Error
    {
        get => GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    public static readonly AttachedProperty<string?> ErrorProperty =
        AvaloniaProperty.RegisterAttached<ErrorBehavior, Control, string?>("Error");

    private static void OnErrorPropertyChanged(AvaloniaPropertyChangedEventArgs<string?> e)
    {
        if (e.Sender is ErrorBehavior behavior)
            behavior.OnErrorChanged(e.NewValue.Value);
    }

    private void OnErrorChanged(string? error)
    {
        if (AssociatedObject is null)
            return;

        if (string.IsNullOrEmpty(error) || !AssociatedObject.IsVisible)
        {
            RemoveErrorAdorner();
        }
        else
        {
            AddErrorAdorner();
        }

        ToolTip.SetTip(AssociatedObject, error);
    }

    // TODO
    private void AddErrorAdorner()
    {
        // if (AssociatedObject is null)
        //     return;
        //
        // var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        // if (adornerLayer is null)
        //     return;
        //        
        // var adorner = adornerLayer.GetAdorners(AssociatedObject)
        //     ?.OfType<ErrorAdorner>()
        //     .FirstOrDefault();
        //
        // if (adorner is null)
        // {
        //     adorner = new ErrorAdorner(AssociatedObject);
        //     adornerLayer.Add(adorner);
        // }
    }

    private void RemoveErrorAdorner()
    {
        // if (AssociatedObject is null)
        //     return;
        //
        // var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        // if (adornerLayer is null)
        //     return;
        //
        // var adorner = adornerLayer.GetAdorners(AssociatedObject)
        //     ?.OfType<ErrorAdorner>()
        //     .FirstOrDefault();
        //
        // if (adorner != null)
        //     adornerLayer.Remove(adorner);
    }

    // private class ErrorAdorner : Adorner
    // {
    //     private readonly Pen _borderPen = new Pen(Brushes.Red, 1);
    //
    //     public ErrorAdorner(UIElement adornedElement)
    //         : base(adornedElement)
    //     {
    //     }
    //
    //     protected override void OnRender(DrawingContext drawingContext)
    //     {
    //         base.OnRender(drawingContext);
    //         var rect = new Rect(new Point(0, 0), AdornedElement.RenderSize);
    //         drawingContext.DrawRectangle(null, _borderPen, rect);
    //     }
    // }
}