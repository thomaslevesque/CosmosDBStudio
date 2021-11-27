using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CosmosDBStudio.ViewModel.Dialogs;

namespace CosmosDBStudio.Avalonia.Services.Implementation;

public partial class DialogWindow : Window
{
    private static readonly FieldInfo DialogResultField;

    static DialogWindow()
    {
        DialogResultField = typeof(Window).GetField("_dialogResult", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("_dialogResult field not found");
    }
    
    public DialogWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DialogButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: DialogButton button })
        {
            if (button.Command is not null)
                return;

            if (button.DialogResult is bool dialogResult)
                this.Close(dialogResult);            
        }
    }

    public object? DialogResult => DialogResultField.GetValue(this);
}