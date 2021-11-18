using CosmosDBStudio.Dialogs;
using EssentialMVVM;
using System;
using System.Diagnostics;
using System.Windows.Input;
using CosmosDBStudio.Services;

namespace CosmosDBStudio.ViewModel
{
    public class AboutViewModel : DialogViewModelBase
    {
        public AboutViewModel(IApplication application)
        {
            AddOkButton();
            try
            {
                var info = application.GetVersionInfo();
                ProductName = info.ProductName;
                Version = info.Version;
                Author = info.Author;
            }
            catch(InvalidOperationException)
            {
                ProductName = "Cosmos DB Studio";
                Version = "X.X.X.X";
                Author = "Thomas Levesque";
            }
            Title = "About " + ProductName;
        }

        public string ProductName { get; }
        public string Version { get; }
        public string Author { get; }

        private DelegateCommand? _openWebsiteCommand;
        public ICommand OpenWebsiteCommand => _openWebsiteCommand ??= new DelegateCommand(OpenWebsite);

        private void OpenWebsite()
        {
            Process.Start(new ProcessStartInfo("https://github.com/thomaslevesque/CosmosDBStudio")
            {
                UseShellExecute = true
            });
        }
    }
}
