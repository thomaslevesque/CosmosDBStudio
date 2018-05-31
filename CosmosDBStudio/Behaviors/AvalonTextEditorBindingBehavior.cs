using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

namespace CosmosDBStudio.Behaviors
{
    public class AvalonTextEditorBindingBehavior : Behavior<TextEditor>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObjectTextChanged;
            AssociatedObject.TextArea.SelectionChanged += AssociatedObjectSelectionChanged;
        }
        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= AssociatedObjectTextChanged;
            base.OnDetaching();
            AssociatedObject.TextArea.SelectionChanged -= AssociatedObjectSelectionChanged;
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(AvalonTextEditorBindingBehavior),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AvalonTextEditorBindingBehavior b)
            {
                b.OnTextChanged();
            }
        }

        private void OnTextChanged()
        {
            if (AssociatedObject.Text != Text)
                AssociatedObject.Text = Text;
        }

        private void AssociatedObjectTextChanged(object sender, EventArgs e)
        {
            if (Text != AssociatedObject.Text)
                Text = AssociatedObject.Text;
        }

        public string SelectedText
        {
            get => (string)GetValue(SelectedTextProperty);
            set => SetValue(SelectedTextProperty, value);
        }

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register(
                "SelectedText",
                typeof(string),
                typeof(AvalonTextEditorBindingBehavior),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedTextPropertyChanged));

        private static void OnSelectedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AvalonTextEditorBindingBehavior b)
            {
                b.OnSelectedTextChanged();
            }
        }

        private void OnSelectedTextChanged()
        {
            if (AssociatedObject.SelectedText != SelectedText)
                AssociatedObject.SelectedText = SelectedText;
        }

        private void AssociatedObjectSelectionChanged(object sender, EventArgs e)
        {
            if (SelectedText != AssociatedObject.SelectedText)
                SelectedText = AssociatedObject.SelectedText;
        }

    }
}