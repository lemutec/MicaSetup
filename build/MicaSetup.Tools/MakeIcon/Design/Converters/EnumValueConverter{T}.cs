using System;
using System.Globalization;
using System.Windows.Data;

namespace MakeIcon.Design.Converters;

public class EnumValueConverter<T> : IValueConverter where T : struct
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return false;
        }
        return Enum.TryParse(value.ToString(), out T enumValue) && Equals(enumValue, parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null!;
        }
        return Enum.Parse(typeof(T), parameter.ToString()!);
    }
}
