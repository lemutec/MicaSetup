using System;
using System.Globalization;
using System.Windows.Data;

namespace MicaSetup.Controls.Converters;

internal class ProgressBarIndeterminateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double @double && @double < 0d)
        {
            return true;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
