using Avalonia;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.Search;

namespace CosmosDBStudio.Avalonia.Behaviors;

public class AvalonTextEditorBehavior : Behavior<TextEditor>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        OnUseSearchChanged();
    }

    public bool UseSearch
    {
        get => (bool)GetValue(UseSearchProperty);
        set => SetValue(UseSearchProperty, value);
    }

    public static readonly AttachedProperty<bool> UseSearchProperty =
        AvaloniaProperty.RegisterAttached<AvalonTextEditorBehavior, TextEditor, bool>(
            nameof(UseSearch),
            defaultValue: false);

    private static void OnUseSearchPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is AvalonTextEditorBehavior behavior)
        {
            behavior.OnUseSearchChanged();
        }
    }

    private SearchPanel? _searchPanel;
    private void OnUseSearchChanged()
    {
        if (AssociatedObject is null)
            return;

        if (UseSearch)
        {
            _searchPanel ??= SearchPanel.Install(AssociatedObject.TextArea);
        }
        else if (_searchPanel != null)
        {
            _searchPanel.Uninstall();
            _searchPanel = null;
        }
    }
}