using System.Windows;
using System.Windows.Controls;
using CosmosDBStudio.ViewModel.Dialogs;

namespace CosmosDBStudio.View
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        private void DialogButton_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var button = (DialogButton)b.DataContext;

            if (button.Command is object)
                return;

            if (button.DialogResult is bool dialogResult)
                DialogResult = dialogResult;
        }
    }
}
