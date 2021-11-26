using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.ViewModel;

namespace CosmosDBStudio.Avalonia
{
    public class App : Application, IApplication
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public App()
        {
        }
        
        public App(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = _mainWindowViewModel;
            }

            base.OnFrameworkInitializationCompleted();
        }

        public void Quit()
        {
            ((IControlledApplicationLifetime)ApplicationLifetime).Shutdown();
        }

        public ApplicationVersionInfo GetVersionInfo()
        {
            return new ApplicationVersionInfo("Cosmos DB Studio", "9.9.9.9", "Thomas Levesque");
        }

        public Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow;
    }
}