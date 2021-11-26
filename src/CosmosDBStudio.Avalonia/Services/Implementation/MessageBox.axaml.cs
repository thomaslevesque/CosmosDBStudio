using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CosmosDBStudio.Avalonia.Services.Implementation;

public partial class MessageBox : Window
{
    private readonly MessageBoxResult _defaultResult;

    public MessageBox()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private MessageBox(string text, string title, MessageBoxButton buttons)
    {
        AvaloniaXamlLoader.Load(this);
        var txtContent = this.FindControl<TextBlock>("txtContent");
        var btnOK = this.FindControl<Button>("btnOK");
        var btnYes = this.FindControl<Button>("btnYes");
        var btnNo = this.FindControl<Button>("btnNo");
        var btnCancel = this.FindControl<Button>("btnCancel");

        Title = title;
        txtContent!.Text = text;

        switch (buttons)
        {
            case MessageBoxButton.OK:
                btnOK!.IsVisible = true;
                btnOK.IsDefault = true;
                _defaultResult = MessageBoxResult.OK;
                break;
            case MessageBoxButton.OKCancel:
                btnOK!.IsVisible = true;
                btnOK.IsDefault = true;
                btnCancel!.IsVisible = true;
                btnCancel.IsCancel = true;
                _defaultResult = MessageBoxResult.Cancel;
                break;
            case MessageBoxButton.YesNoCancel:
                btnYes!.IsVisible = true;
                btnYes.IsDefault = true;
                btnNo!.IsVisible = true;
                btnCancel!.IsVisible = true;
                btnCancel.IsCancel = true;
                _defaultResult = MessageBoxResult.Cancel;
                break;
            case MessageBoxButton.YesNo:
                btnYes!.IsVisible = true;
                btnYes.IsDefault = true;
                btnNo!.IsVisible = true;
                btnNo.IsCancel = true;
                _defaultResult = MessageBoxResult.No;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
        }
    }

    public static async Task<MessageBoxResult> Show(string text, string title, MessageBoxButton buttons, Window parent)
    {
        var box = new MessageBox(text, title, buttons);
        var result = await box.ShowDialog<MessageBoxResult>(parent);
        if (result == MessageBoxResult.None)
            result = box._defaultResult;

        return result;
    }

    private void BtnOK_OnClick(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.OK);
    private void BtnCancel_OnClick(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.Cancel);
    private void BtnYes_OnClick(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.Yes);
    private void BtnNo_OnClick(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.No);
}

public enum MessageBoxButton
{
    OK,
    OKCancel,
    YesNoCancel,
    YesNo
}

public enum MessageBoxResult
{
    None,
    OK,
    Cancel,
    Yes,
    No
}