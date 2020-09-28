using CosmosDBStudio.Extensions;
using CosmosDBStudio.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace CosmosDBStudio.View
{
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {
            InitializeComponent();
        }

        private void TreeViewItem_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var vm = (e.Source as FrameworkElement)?.DataContext as TreeNodeViewModel;
            if (vm != null)
            {
                vm.DefaultCommand.TryExecute(vm.DefaultCommandParameter);
            }
            e.Handled = true;
        }
    }
}
