using CosmosDBStudio.SyntaxHighlighting;
using CosmosDBStudio.ViewModel;
using System.Windows;
using CosmosDBStudio.Services;

namespace CosmosDBStudio
{
    public partial class App : IApplication
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

        public ApplicationVersionInfo GetVersionInfo()
        {
            var package = global::Windows.ApplicationModel.Package.Current;
            var v = package.Id.Version;
            return new ApplicationVersionInfo(package.DisplayName, $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}", package.PublisherDisplayName);
        }
    }
}
