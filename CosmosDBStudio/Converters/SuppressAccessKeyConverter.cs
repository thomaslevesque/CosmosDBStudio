using System;
using System.Globalization;
using System.Windows.Data;

namespace CosmosDBStudio.Converters
{
    public class SuppressAccessKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                int underscore = s.IndexOf('_');
                if (underscore < 0)
                    return s;
                return s.Insert(underscore, "_");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
