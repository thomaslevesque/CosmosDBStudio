using System.Windows;
using System.Windows.Media;

namespace CosmosDBStudio.Extensions
{
    public static class UIElementExtensions
    {
        public static TAncestor? GetAncestorOrSelf<TAncestor>(this UIElement element)
            where TAncestor : UIElement
        {
            UIElement? uiElement = element;
            while ((uiElement != null) && !(uiElement is TAncestor))
                uiElement = VisualTreeHelper.GetParent(uiElement) as UIElement;

            return uiElement as TAncestor;
        }
    }
}
