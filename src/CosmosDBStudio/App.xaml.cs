using CosmosDBStudio.ViewModel;
using System.Windows;

namespace CosmosDBStudio
{
    public partial class App
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public App(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            _mainWindowViewModel = mainWindowViewModel;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.DataContext = _mainWindowViewModel;
            MainWindow.Show();
        }
    }
}
