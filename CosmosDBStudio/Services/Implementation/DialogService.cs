using CosmosDBStudio.Dialogs;
using CosmosDBStudio.View;
using System.Windows;
using System.Windows.Data;

namespace CosmosDBStudio.Services.Implementation
{
    public class DialogService : IDialogService
    {
        public bool Confirm(string text)
        {
            var result = MessageBox.Show(text, "Confirm", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        public bool? ShowDialog(IDialogViewModel dialog)
        {
            var window = CreateWindow(dialog);
            try
            {
                dialog.CloseRequested += OnCloseRequested;
                return window.ShowDialog();
            }
            finally
            {
                dialog.CloseRequested -= OnCloseRequested;
            }

            void OnCloseRequested(object? sender, bool? dialogResult)
            {
                if (dialogResult is bool value)
                    window.DialogResult = value;
                else
                    window.Close();
            }
        }

        private DialogWindow CreateWindow(IDialogViewModel dialog)
        {
            var window = new DialogWindow();

            BindingOperations.SetBinding(
                window,
                Window.TitleProperty,
                new Binding(nameof(IDialogViewModel.Title)));

            if (dialog is ISizableDialog sizable)
            {
                if (sizable.IsResizable)
                {
                    BindingOperations.SetBinding(
                        window,
                        FrameworkElement.WidthProperty,
                        new Binding(nameof(ISizableDialog.Width))
                        {
                            Mode = BindingMode.TwoWay
                        });
                    BindingOperations.SetBinding(
                        window,
                        FrameworkElement.HeightProperty,
                        new Binding(nameof(ISizableDialog.Height))
                        {
                            Mode = BindingMode.TwoWay
                        });
                    window.ResizeMode = ResizeMode.CanResizeWithGrip;
                }
                else
                {
                    window.Width = sizable.Width;
                    window.Height = sizable.Height;
                }
            }
            else
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            }

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.DataContext = dialog;
            return window;
        }
    }
}
