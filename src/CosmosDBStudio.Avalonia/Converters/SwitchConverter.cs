using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CosmosDBStudio.Avalonia.Converters;

class SwitchConverter : IValueConverter
{
    public object? TrueValue { get; set; }
    public object? FalseValue { get; set; }
    public object? FallbackValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true)
            return TrueValue;
        if (value is false)
            return FalseValue;
        return FallbackValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}