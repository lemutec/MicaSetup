using System;
using System.Globalization;
using Property = System.Windows.DependencyProperty;

namespace MicaSetup.Design.Converters;

public abstract class ValueToBoolConverterBase<T, TConverter> : ConverterBase
    where TConverter : new()
{
    public abstract T TrueValue { get; set; }

    public bool IsInverted
    {
        get => (bool)GetValue(IsInvertedProperty);
        set => SetValue(IsInvertedProperty, value);
    }

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var trueValue = TrueValue;
        return Equals(value, trueValue) ^ IsInverted;
    }

    public static readonly Property IsInvertedProperty = PropertyHelper.Create<bool, ValueToBoolConverterBase<T, TConverter>>(nameof(IsInverted));
}
