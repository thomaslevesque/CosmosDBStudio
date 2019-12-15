using Microsoft.Xaml.Behaviors;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CosmosDBStudio.Behaviors
{
    public class ErrorBehavior : Behavior<FrameworkElement>
    {
        protected override void OnDetaching()
        {
            RemoveErrorAdorner();
            base.OnDetaching();
        }

        public string? Error
        {
            get { return (string?)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }

        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register(
                "Error",
                typeof(string),
                typeof(ErrorBehavior),
                new PropertyMetadata(OnErrorPropertyChanged));

        private static void OnErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ErrorBehavior behavior)
                behavior.OnErrorChanged((string?)e.NewValue);
        }

        private void OnErrorChanged(string? error)
        {
            if (string.IsNullOrEmpty(error))
            {
                RemoveErrorAdorner();
            }
            else
            {
                AddErrorAdorner();
            }

            AssociatedObject.ToolTip = error;
        }

        private void AddErrorAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            var adorner = adornerLayer.GetAdorners(AssociatedObject)
                    ?.OfType<ErrorAdorner>()
                    .FirstOrDefault();

            if (adorner is null)
            {
                adorner = new ErrorAdorner(AssociatedObject);
                adornerLayer.Add(adorner);
            }
        }

        private void RemoveErrorAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            var adorner = adornerLayer.GetAdorners(AssociatedObject)
                    ?.OfType<ErrorAdorner>()
                    .FirstOrDefault();

            if (adorner != null)
                adornerLayer.Remove(adorner);
        }

        private class ErrorAdorner : Adorner
        {
            private readonly Pen _borderPen = new Pen(Brushes.Red, 1);

            public ErrorAdorner(UIElement adornedElement)
                : base(adornedElement)
            {
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                var rect = new Rect(new Point(0, 0), AdornedElement.RenderSize);
                drawingContext.DrawRectangle(null, _borderPen, rect);
            }
        }
    }
}
