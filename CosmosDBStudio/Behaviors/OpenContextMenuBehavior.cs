using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CosmosDBStudio.Behaviors
{
    public class OpenContextMenuBehavior : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= AssociatedObject_Click;
            base.OnDetaching();
        }

        private void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AssociatedObject.ContextMenu is ContextMenu menu)
            {
                menu.PlacementTarget = AssociatedObject;
                menu.IsOpen = true;
            }
        }
    }
}
