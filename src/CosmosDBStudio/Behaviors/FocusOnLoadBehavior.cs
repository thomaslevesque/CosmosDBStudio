using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace CosmosDBStudio.Behaviors
{
    public class FocusOnLoadBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Focus();
        }
    }
}
