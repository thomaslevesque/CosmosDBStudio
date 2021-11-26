using System;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Styling;
using Avalonia.Threading;

namespace CosmosDBStudio.Avalonia.View.Controls;

public class AccountExplorerTreeView : TreeView, IStyleable
{
    Type IStyleable.StyleKey => typeof(TreeView);

    protected override IItemContainerGenerator CreateItemContainerGenerator()
    {
        var result = new TreeItemContainerGenerator<AccountExplorerTreeViewItem>(
            this,
            AccountExplorerTreeViewItem.HeaderProperty,
            AccountExplorerTreeViewItem.ItemTemplateProperty,
            AccountExplorerTreeViewItem.ItemsProperty,
            TreeViewItem.IsExpandedProperty);
        result.Index.Materialized += ContainerMaterialized;
        return result;
    }
    
    private void ContainerMaterialized(object? sender, ItemContainerEventArgs e)
    {
        var selectedItem = SelectedItem;

        if (selectedItem == null)
        {
            return;
        }

        foreach (var container in e.Containers)
        {
            if (container.Item == selectedItem)
            {
                ((TreeViewItem)container.ContainerControl).IsSelected = true;

                if (AutoScrollToSelectedItem)
                {
                    Dispatcher.UIThread.Post(container.ContainerControl.BringIntoView);
                }

                break;
            }
        }
    }
}