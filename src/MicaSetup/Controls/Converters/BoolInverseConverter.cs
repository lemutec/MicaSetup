﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace MicaSetup.Controls.Converters;

internal sealed class BoolInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }
}
