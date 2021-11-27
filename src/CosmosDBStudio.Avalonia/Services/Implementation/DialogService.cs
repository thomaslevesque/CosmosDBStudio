using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Threading;
using CosmosDBStudio.ViewModel.Dialogs;
using CosmosDBStudio.ViewModel.Services;
using Hamlet;

namespace CosmosDBStudio.Avalonia.Services.Implementation;

public class DialogService : IDialogService
{
    public bool? ShowDialog(IDialogViewModel dialog)
    {
        var window = CreateWindow(dialog);
        window.Closing += OnWindowClosing;
        bool? result = null;
        try
        {
            dialog.CloseRequested += OnCloseRequested;
            result = ShowDialogSync(window.ShowDialog<bool?>(GetDialogOwner()));
            return result;
        }
        finally
        {
            dialog.OnClosed(result);
            dialog.CloseRequested -= OnCloseRequested;
            window.Closing -= OnWindowClosing;
        }

        void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            e.Cancel = !ConfirmClose((bool?)window.DialogResult);
        }

        void OnCloseRequested(object? sender, bool? dialogResult)
        {
            if (dialogResult is bool value)
                window.Close(value);
            else
                window.Close();
        }

        bool ConfirmClose(bool? dialogResult)
        {
            var closingEventArgs = new DialogClosingEventArgs(dialogResult);
            dialog.OnClosing(closingEventArgs);
            return !closingEventArgs.Cancel;
        }
    }

    public bool Confirm(string text)
    {
        var result = ShowDialogSync(MessageBox.Show(text, "Confirm", MessageBoxButton.YesNo, GetDialogOwner()));
        return result == MessageBoxResult.Yes;
    }

    public Option<bool> YesNoCancel(string text)
    {
        var result = ShowDialogSync(MessageBox.Show(text, "Confirm", MessageBoxButton.YesNoCancel, GetDialogOwner()));
        return result switch
        {
            MessageBoxResult.Yes => Option.Some(true),
            MessageBoxResult.No => Option.Some(false),
            _ => Option.None()
        };
    }

    public void ShowError(string message)
    {
        ShowDialogSync(MessageBox.Show(message, "Error", MessageBoxButton.OK, GetDialogOwner()));
    }

    public Option<string> PickFileToSave(Option<string> filter = default, Option<int> filterIndex = default, Option<string> fileName = default,
        Option<string> initialDirectory = default)
    {
        var dialog = new SaveFileDialog();
        filter.Do(value => dialog.Filters = ParseFileDialogFilters(value).ToList());
        filterIndex.Do(value =>
        {
            var f = dialog.Filters.ElementAtOrDefault(value);
            if (f is not null && f.Extensions.Any())
                dialog.DefaultExtension = f.Extensions[0];
        });
        fileName.Do(value => dialog.InitialFileName = value);
        initialDirectory.Do(value => dialog.Directory = value);


        var result = ShowDialogSync(dialog.ShowAsync(GetDialogOwner()));
        if (result is null)
            return Option.None();

        return result;
    }

    public Option<string> PickFileToOpen(Option<string> filter = default, Option<int> filterIndex = default, Option<string> fileName = default,
        Option<string> initialDirectory = default)
    {
        var dialog = new OpenFileDialog();
        dialog.AllowMultiple = false;
        filter.Do(value => dialog.Filters = ParseFileDialogFilters(value).ToList());
        fileName.Do(value => dialog.InitialFileName = value);
        initialDirectory.Do(value => dialog.Directory = value);

        var result = ShowDialogSync(dialog.ShowAsync(GetDialogOwner()));
        if (result is null || result.Length == 0)
            return Option.None();

        return result[0];
    }
    
    public Option<string> TextPrompt(string prompt, Option<string> initialText = default)
    {
        throw new NotImplementedException();
    }
    
    private T ShowDialogSync<T>(Task<T> dialogTask)
    {
        using (var cts = new CancellationTokenSource())
        {
            dialogTask.ContinueWith(t =>
            {
                // Observe exception
                _ = t.Exception;
                cts.Cancel();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(cts.Token);
            return dialogTask.Result;
        }
    }

    // TODO: not always MainWindow...
    private Window GetDialogOwner() => ((App)Application.Current).MainWindow;

    private static IEnumerable<FileDialogFilter> ParseFileDialogFilters(string filter)
    {
        var parts = filter.Split('|');
        for (int i = 0; i < parts.Length; i += 2)
        {
            if (i + 1 >= parts.Length)
                break;
            var name = parts[i];
            var extensions = parts[i + 1].Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            yield return new FileDialogFilter { Name = name, Extensions = extensions };
        }
    }
    
    private DialogWindow CreateWindow(IDialogViewModel dialog)
    {
        var window = new DialogWindow();
        window.DataContext = dialog;

        //window.Bind(Window.TitleProperty, new Binding(nameof(IDialogViewModel.Title)));

        if (dialog is ISizableDialog sizable)
        {
            if (sizable.IsResizable)
            {
                window.Bind(Layoutable.WidthProperty, new Binding(nameof(ISizableDialog.Width))
                {
                    Mode = BindingMode.TwoWay
                });
                window.Bind(Layoutable.HeightProperty, new Binding(nameof(ISizableDialog.Height))
                {
                    Mode = BindingMode.TwoWay
                });
                window.CanResize = true;
            }
            else
            {
                window.Width = sizable.Width;
                window.Height = sizable.Height;
                window.CanResize = false;
            }
        }
        else
        {
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.CanResize = false;
        }

        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        return window;
    }
}