using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CosmosDBStudio.View.Controls
{
    public class TreeNodeControl : ContentControl
    {
        static TreeNodeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TreeNodeControl),
                new FrameworkPropertyMetadata(typeof(TreeNodeControl)));
        }

        public ImageSource Icon
        {
            get => (ImageSource) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TreeNodeControl),
                new PropertyMetadata(null));
    }
}