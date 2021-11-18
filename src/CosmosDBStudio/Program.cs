using CosmosDBStudio.Commands;
using CosmosDBStudio.Extensions;
using CosmosDBStudio.Model;
using CosmosDBStudio.Services;
using CosmosDBStudio.Services.Implementation;
using CosmosDBStudio.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CosmosDBStudio
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var serviceProvider = CreateServiceProvider();
            var app = serviceProvider.GetRequiredService<App>();
            app.Run();
        }

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
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton(sp => sp.GetRequiredService<App>().Dispatcher);
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