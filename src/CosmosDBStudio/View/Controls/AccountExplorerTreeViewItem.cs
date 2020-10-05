using CosmosDBStudio.Extensions;
using CosmosDBStudio.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CosmosDBStudio.View.Controls
{
    public class AccountExplorerTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AccountExplorerTreeViewItem();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var clickedTreeViewItem = (e.OriginalSource as UIElement)?.GetAncestorOrSelf<AccountExplorerTreeViewItem>();
            if (clickedTreeViewItem != this)
                return;

            e.Handled = TryExecuteDefaultCommand();
            base.OnMouseDoubleClick(e);
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
}
