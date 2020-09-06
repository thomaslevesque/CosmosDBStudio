using CosmosDBStudio.ViewModel;
using System.ComponentModel;

namespace CosmosDBStudio
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (App.Current.IsShuttingDown)
                return;

            e.Cancel = true;
            if (DataContext is MainWindowViewModel vm)
            {
                vm.QuitCommand.Execute(null);
            }
        }
    }
}
