using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View.Controls
{
    public class AccountExplorerTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AccountExplorerTreeViewItem();
        }
    }
}
