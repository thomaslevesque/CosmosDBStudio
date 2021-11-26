using System;
using Avalonia;
using Avalonia.Input.Platform;
using CosmosDBStudio.Avalonia.Services.Implementation;
using CosmosDBStudio.Model;
using CosmosDBStudio.Model.Services;
using CosmosDBStudio.Model.Services.Implementation;
using CosmosDBStudio.Util.Extensions;
using CosmosDBStudio.ViewModel;
using CosmosDBStudio.ViewModel.Commands;
using CosmosDBStudio.ViewModel.Services;
using CosmosDBStudio.ViewModel.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace CosmosDBStudio.Avalonia
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            IconProvider.Register<FontAwesomeIconProvider>();
            var services = CreateServiceProvider();
            BuildAvaloniaApp(services)
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp(IServiceProvider services)
            => AppBuilder.Configure(services.GetRequiredService<App>)
                .UsePlatformDetect()
                .LogToTrace();
        
        private static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAccountDirectory, AccountDirectory>();
            services.AddSingleton<IClientPool, ClientPool>();
            services.AddSingleton<IQueryPersistenceService, QueryPersistenceService>();
            services.AddSingleton<IAccountContextFactory, AccountContextFactory>();

            services.AddSingleton<IViewModelFactory>(ViewModelFactoryProxy.Create);
            services.AddSingleton<IMessenger, Messenger>();
            
            // UI Services
            // TODO: DialogService
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IUIDispatcher, UIDispatcher>();
            services.AddSingleton<IClipboardService, ClipboardService>();

            services.AddLazyResolution();

            services.AddSingleton<AccountCommands>();
            services.AddSingleton<DatabaseCommands>();
            services.AddSingleton<ContainerCommands>();
            services.AddSingleton<ScriptCommands<CosmosStoredProcedure>>();
            services.AddSingleton<ScriptCommands<CosmosUserDefinedFunction>>();
            services.AddSingleton<ScriptCommands<CosmosTrigger>>();

            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<App>();
            services.AddSingleton<IApplication>(sp => sp.GetRequiredService<App>());
        }
    }
}