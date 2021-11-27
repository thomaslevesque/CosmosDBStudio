using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CosmosDBStudio.Avalonia.Converters;

public class NotConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;

        return BindingOperations.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}