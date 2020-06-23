using System;
using System.Windows;
using System.Windows.Markup;

namespace CosmosDBStudio.Markup
{
    public class EnumValuesExtension : MarkupExtension
    {
        public EnumValuesExtension()
        {
        }

        public EnumValuesExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public Type? EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType is null || !EnumType.IsEnum)
                return DependencyProperty.UnsetValue;

            return Enum.GetValues(EnumType);
        }
    }
}
