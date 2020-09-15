using CosmosDBStudio.Dialogs;
using EssentialMVVM;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CosmosDBStudio.ViewModel
{
    public class AboutViewModel : DialogViewModelBase
    {
        public AboutViewModel()
        {
            AddOkButton();
            try
            {
                var package = Windows.ApplicationModel.Package.Current;
                ProductName = package.DisplayName;
                var v = package.Id.Version;
                Version = $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
                Author = package.PublisherDisplayName;
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
