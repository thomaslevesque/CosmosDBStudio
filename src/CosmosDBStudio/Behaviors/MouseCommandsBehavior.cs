using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CosmosDBStudio.Behaviors
{
    public class MouseCommandsBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDoubleClick += OnMouseDoubleClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDoubleClick -= OnMouseDoubleClick;
            base.OnDetaching();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TryExecute(DoubleClickCommand, DoubleClickCommandParameter);
        }

        private static void TryExecute(ICommand? command, object? parameter)
        {
            if (command is null)
                return;
            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }

        public ICommand? DoubleClickCommand
        {
            get { return (ICommand)GetValue(MouseDoubleClickProperty); }
            set { SetValue(MouseDoubleClickProperty, value); }
        }

        public static readonly DependencyProperty MouseDoubleClickProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(MouseCommandsBehavior), new PropertyMetadata(null));

        public object DoubleClickCommandParameter
        {
            get { return (object)GetValue(DoubleClickCommandParameterProperty); }
            set { SetValue(DoubleClickCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.Register("DoubleClickCommandParameter", typeof(object), typeof(MouseCommandsBehavior), new PropertyMetadata(null));
    }
}
