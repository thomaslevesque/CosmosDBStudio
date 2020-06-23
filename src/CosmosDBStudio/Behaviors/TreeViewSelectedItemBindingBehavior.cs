using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.Behaviors
{
    public class TreeViewSelectedItemBindingBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += AssociatedItemSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectedItemChanged -= AssociatedItemSelectedItemChanged;
            base.OnDetaching();
        }

        /// <summary>
        /// Note: one-way-to-source only
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(TreeViewSelectedItemBindingBehavior),
                new PropertyMetadata(null));

        private void AssociatedItemSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            if (SelectedItem != args.NewValue)
                this.SetCurrentValue(SelectedItemProperty, args.NewValue);
        }
    }
}
