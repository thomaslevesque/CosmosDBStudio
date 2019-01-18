using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View.Controls
{
    public class QuerySheetTabControl : TabControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is QuerySheetView;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new QuerySheetView();
        }
    }
}
