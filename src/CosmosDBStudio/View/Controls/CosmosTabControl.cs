using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View.Controls
{
    public class CosmosTabControl : TabControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CosmosTabItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CosmosTabItem();
        }
    }
}
