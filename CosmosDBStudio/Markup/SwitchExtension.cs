using System;
using System.Globalization;
using System.Windows.Data;

namespace CosmosDBStudio.Markup
{
    public class SwitchExtension : Binding
    {
        public SwitchExtension()
        {
            Converter = new SwitchConverter(this);
        }

        public SwitchExtension(string path)
            : base(path)
        {
            Converter = new SwitchConverter(this);
        }

        public object? TrueValue { get; set; }
        public object? FalseValue { get; set; }

        private class SwitchConverter : IValueConverter
        {
            private readonly SwitchExtension _switch;

            public SwitchConverter(SwitchExtension @switch)
            {
                _switch = @switch;
            }

            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                if (value is true)
                    return _switch.TrueValue;
                if (value is false)
                    return _switch.FalseValue;
                return _switch.FallbackValue;
            }

            public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
