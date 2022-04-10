using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;

namespace CosmosDBStudio.Avalonia.Behaviors;

public class AvalonTextEditorBindingBehavior : Behavior<TextEditor>
{
    static AvalonTextEditorBindingBehavior()
    {
        TextProperty.Changed.Subscribe(OnTextPropertyChanged);
        SelectedTextProperty.Changed.Subscribe(OnSelectedTextPropertyChanged);
        SelectionProperty.Changed.Subscribe(OnSelectionPropertyChanged);
        CursorPositionProperty.Changed.Subscribe(OnCursorPositionPropertyChanged);
    }
    
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.TextChanged += AssociatedObjectTextChanged;
        AssociatedObject.TextArea.SelectionChanged += AssociatedObjectSelectionChanged;
        AssociatedObject.TextArea.Caret.PositionChanged += AssociatedObjectCaretPositionChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.TextChanged -= AssociatedObjectTextChanged;
        AssociatedObject.TextArea.SelectionChanged -= AssociatedObjectSelectionChanged;
        AssociatedObject.TextArea.Caret.PositionChanged -= AssociatedObjectCaretPositionChanged;
        base.OnDetaching();
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly AttachedProperty<string> TextProperty =
        AvaloniaProperty.RegisterAttached<AvalonTextEditorBindingBehavior, TextEditor, string>(
            nameof(Text),
            defaultBindingMode: BindingMode.TwoWay,
            coerce: (_, value) => value ?? string.Empty);

    private static void OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs<string> e)
    {
        if (e.Sender is AvalonTextEditorBindingBehavior b)
        {
            b.OnTextChanged();
        }
    }

    private void OnTextChanged()
    {
        if (AssociatedObject is null)
            return;

        if (AssociatedObject.Text != Text)
            AssociatedObject.Text = Text ?? string.Empty;
    }

    private void AssociatedObjectTextChanged(object? sender, EventArgs e)
    {
        if (AssociatedObject is null)
            return;

        if (Text != AssociatedObject.Text)
            Text = AssociatedObject.Text ?? string.Empty;
    }

    public string SelectedText
    {
        get => GetValue(SelectedTextProperty);
        set => SetValue(SelectedTextProperty, value);
    }

    public static readonly AttachedProperty<string> SelectedTextProperty =
        AvaloniaProperty.RegisterAttached<AvalonTextEditorBindingBehavior, TextEditor, string>(
            nameof(SelectedText),
            defaultBindingMode: BindingMode.TwoWay);

    private static void OnSelectedTextPropertyChanged(AvaloniaPropertyChangedEventArgs<string> e)
    {
        if (e.Sender is AvalonTextEditorBindingBehavior b)
        {
            b.OnSelectedTextChanged();
        }
    }

    private void OnSelectedTextChanged()
    {
        if (AssociatedObject is null)
            return;

        if (AssociatedObject.SelectedText != SelectedText)
            AssociatedObject.SelectedText = SelectedText ?? string.Empty;
    }

    public (int start, int length) Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }

    public static readonly AttachedProperty<(int start, int length)> SelectionProperty =
        AvaloniaProperty.RegisterAttached<AvalonTextEditorBindingBehavior, TextEditor, (int start, int length)>(
            nameof(Selection),
            defaultValue: (0, 0),
            defaultBindingMode: BindingMode.TwoWay);

    private static void OnSelectionPropertyChanged(AvaloniaPropertyChangedEventArgs<(int start, int length)> e)
    {
        if (e.Sender is AvalonTextEditorBindingBehavior b)
        {
            b.OnSelectionChanged();
        }
    }

    private void OnSelectionChanged()
    {
        if (AssociatedObject is null)
            return;

        var associatedObjectSelection = (AssociatedObject.SelectionStart, AssociatedObject.SelectionLength);
        if (associatedObjectSelection != Selection)
        {
            var (start, end) = Selection;
            AssociatedObject.Select(start, end);
        }
    }

    private void AssociatedObjectSelectionChanged(object? sender, EventArgs e)
    {
        if (AssociatedObject is null)
            return;

        if (SelectedText != AssociatedObject.SelectedText)
            SelectedText = AssociatedObject.SelectedText ?? string.Empty;
        var associatedObjectSelection = (AssociatedObject.SelectionStart, AssociatedObject.SelectionLength);
        if (Selection != associatedObjectSelection)
            Selection = associatedObjectSelection;
    }

    public int CursorPosition
    {
        get => GetValue(CursorPositionProperty);
        set => SetValue(CursorPositionProperty, value);
    }

    public static readonly AttachedProperty<int> CursorPositionProperty =
        AvaloniaProperty.RegisterAttached<AvalonTextEditorBindingBehavior, TextEditor, int>(
            nameof(CursorPosition),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

    private static void OnCursorPositionPropertyChanged(AvaloniaPropertyChangedEventArgs<int> e)
    {
        if (e.Sender is AvalonTextEditorBindingBehavior b)
        {
            b.OnCursorPositionChanged();
        }
    }

    private void OnCursorPositionChanged()
    {
        if (AssociatedObject is null)
            return;

        if (AssociatedObject.CaretOffset != CursorPosition)
            AssociatedObject.CaretOffset = CursorPosition;
    }

    private void AssociatedObjectCaretPositionChanged(object? sender, EventArgs e)
    {
        if (AssociatedObject is null)
            return;
        
        if (CursorPosition != AssociatedObject.CaretOffset)
            CursorPosition = AssociatedObject.CaretOffset;
    }
}