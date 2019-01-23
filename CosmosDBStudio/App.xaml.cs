using System.Windows;
using CosmosDBStudio.ViewModel;

namespace CosmosDBStudio
{
    public partial class App
    {
        private readonly IViewModelFactory _viewModelFactory;

        public App(IViewModelFactory viewModelFactory)
        {
            InitializeComponent();
            _viewModelFactory = viewModelFactory;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var vm = _viewModelFactory.CreateMainWindowViewModel();
            MainWindow = new MainWindow();
            MainWindow.DataContext = vm;
            MainWindow.Show();
        }
    }
}
