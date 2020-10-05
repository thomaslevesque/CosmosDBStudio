using CosmosDBStudio.SyntaxHighlighting;
using CosmosDBStudio.ViewModel;
using System.Windows;

namespace CosmosDBStudio
{
    public partial class App
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public App(MainWindowViewModel mainWindowViewModel)
        {
            CosmosSyntax.Init();
            InitializeComponent();
            _mainWindowViewModel = mainWindowViewModel;
        }

        public static new App Current => (App)Application.Current;

        public bool IsShuttingDown { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.DataContext = _mainWindowViewModel;
            MainWindow.Show();
        }

        public void Quit()
        {
            IsShuttingDown = true;
            Shutdown();
        }
    }
}
