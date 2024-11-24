using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MicaSetup.Design.Converters;

[ValueConversion(typeof(string), typeof(bool))]
public class StringIsNotNullOrEmptyConverter : StringIsNullOrEmptyConverter
{
    public StringIsNotNullOrEmptyConverter()
    {
        this.IsInverted = true;
    }
}

[ValueConversion(typeof(string), typeof(bool))]
public class StringIsNullOrEmptyConverter : SingletonConverterBase<StringIsNotNullOrEmptyConverter>
{
    public static readonly DependencyProperty IsInvertedProperty = DependencyProperty.Register(
        nameof(IsInverted),
        typeof(bool),
        typeof(StringIsNullOrEmptyConverter),
        new PropertyMetadata(false));

    public bool IsInverted
    {
        get => (bool)GetValue(IsInvertedProperty);
        set => SetValue(IsInvertedProperty, value);
    }

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType is null)
        {
            throw new ArgumentNullException(nameof(targetType));
        }

        if (IsInverted)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        return string.IsNullOrEmpty(value as string);
    }
}
