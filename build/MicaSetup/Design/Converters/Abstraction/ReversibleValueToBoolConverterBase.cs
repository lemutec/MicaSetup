using System;
using System.Globalization;
using Property = System.Windows.DependencyProperty;

namespace MicaSetup.Design.Converters;

public abstract class ReversibleValueToBoolConverterBase<T, TConverter> : ValueToBoolConverterBase<T, TConverter>
    where TConverter : new()
{
    public abstract T FalseValue { get; set; }

    public bool BaseOnFalseValue
    {
        get => (bool)GetValue(BaseOnFalseValueProperty);
        set => SetValue(BaseOnFalseValueProperty, value);
    }

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!BaseOnFalseValue)
        {
            return base.Convert(value, targetType, parameter, culture);
        }

        var falseValue = FalseValue;
        return !Equals(value, falseValue) ^ IsInverted;
    }

    protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return true.Equals(value) ^ IsInverted ? TrueValue! : FalseValue!;
    }

    public static readonly Property BaseOnFalseValueProperty = PropertyHelper.Create<bool, ValueToBoolConverterBase<T, TConverter>>(nameof(BaseOnFalseValueProperty));
}
