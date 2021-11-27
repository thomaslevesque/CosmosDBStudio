using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Styling;

namespace CosmosDBStudio.Avalonia.View.Controls;

[PseudoClasses(":readonly")]
public class TextBoxEx : TextBox, IStyleable
{
    Type IStyleable.StyleKey => typeof(TextBox);

    static TextBoxEx()
    {
        IsReadOnlyProperty.Changed.Subscribe(e =>
        {
            if (e.Sender is TextBoxEx textBox)
            {
                textBox.PseudoClasses.Set(":readonly", e.NewValue.Value);
            }
        });
    }
}