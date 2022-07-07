using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using GxonToolKit.Enums;

namespace GxonToolKit.Helpers;

public class ElementBackdropToBooleanConverter : IValueConverter
{
    public ElementBackdropToBooleanConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string enumString)
        {
            if (!Enum.IsDefined(typeof(ElementBackdrop), value))
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
            }

            var enumValue = Enum.Parse(typeof(ElementBackdrop), enumString);

            return enumValue.Equals(value);
        }

        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string enumString)
        {
            return Enum.Parse(typeof(ElementTheme), enumString);
        }

        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }
}
