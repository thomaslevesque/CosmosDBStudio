using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CosmosDBStudio.Converters
{
    public class EnumToInt32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return DependencyProperty.UnsetValue;

            var type = value.GetType();
            if (!type.IsEnum)
                return DependencyProperty.UnsetValue;

            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!targetType.IsEnum)
                return DependencyProperty.UnsetValue;

            if (value is null)
                return DependencyProperty.UnsetValue;

            return Enum.ToObject(targetType, value);
        }
    }
}
