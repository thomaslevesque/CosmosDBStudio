using CosmosDBStudio.Dialogs;
using CosmosDBStudio.View;
using Hamlet;
using Microsoft.Win32;
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

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public Option<string> PickFileToSave(
            Option<string> filter = default,
            Option<int> filterIndex = default,
            Option<string> fileName = default,
            Option<string> initialDirectory = default)
        {
            return PickFile<SaveFileDialog>(filter, filterIndex, fileName, initialDirectory);
        }

        public Option<string> PickFileToOpen(
            Option<string> filter = default,
            Option<int> filterIndex = default,
            Option<string> fileName = default,
            Option<string> initialDirectory = default)
        {
            return PickFile<OpenFileDialog>(filter, filterIndex, fileName, initialDirectory);
        }

        public bool? ShowDialog(IDialogViewModel dialog)
        {
            var window = CreateWindow(dialog);
            bool? result = null;
            try
            {
                dialog.CloseRequested += OnCloseRequested;
                result = window.ShowDialog();
                return result;
            }
            finally
            {
                dialog.OnClosed(result);
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
            window.DataContext = dialog;

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
                    window.ResizeMode = ResizeMode.NoResize;
                }
            }
            else
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.ResizeMode = ResizeMode.NoResize;
            }

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window;
        }

        private static Option<string> PickFile<TDialog>(
            Option<string> filter,
            Option<int> filterIndex,
            Option<string> fileName,
            Option<string> initialDirectory)
            where TDialog : FileDialog, new()
        {
            var picker = new TDialog();
            filter.Do(value => picker.Filter = value);
            filterIndex.Do(value => picker.FilterIndex = value);
            fileName.Do(value => picker.FileName = value);
            initialDirectory.Do(value => picker.InitialDirectory = value);

            if (picker.ShowDialog() is true)
            {
                return picker.FileName;
            }

            return Option.None();
        }
    }
}
