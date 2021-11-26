using System;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CosmosDBStudio.Avalonia.Extensions;
using CosmosDBStudio.ViewModel.TreeNodes;

namespace CosmosDBStudio.Avalonia.View.Controls;

public class AccountExplorerTreeViewItem : TreeViewItem, IStyleable
{
    Type IStyleable.StyleKey => typeof(TreeViewItem);
    
    public AccountExplorerTreeViewItem()
    {
        Gestures.AddDoubleTappedHandler(this, OnDoubleTapped);
    }

    protected override IItemContainerGenerator CreateItemContainerGenerator()
    {
        return new TreeItemContainerGenerator<AccountExplorerTreeViewItem>(
            this,
            AccountExplorerTreeViewItem.HeaderProperty,
            AccountExplorerTreeViewItem.ItemTemplateProperty,
            AccountExplorerTreeViewItem.ItemsProperty,
            AccountExplorerTreeViewItem.IsExpandedProperty);
    }
    
    private void OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        var clickedTreeViewItem = (e.Source as IVisual)?.FindAncestorOfType<AccountExplorerTreeViewItem>(true);
        if (!ReferenceEquals(clickedTreeViewItem, this))
            return;

        e.Handled = TryExecuteDefaultCommand();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        e.Handled = e.Key == Key.Enter && TryExecuteDefaultCommand();
        base.OnKeyDown(e);
    }

    private bool TryExecuteDefaultCommand()
    {
        if (DataContext is TreeNodeViewModel vm)
        {
            return vm.DefaultCommand.TryExecute(vm.DefaultCommandParameter);
        }

        return false;
    }
}