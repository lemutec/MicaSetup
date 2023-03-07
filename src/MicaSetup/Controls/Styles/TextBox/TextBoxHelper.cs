using System.Windows;

namespace MicaSetup.Controls;

public class TextBoxHelper
{
    public static CornerRadius GetCornerRadius(DependencyObject obj)
    {
        return (CornerRadius)obj.GetValue(CornerRadiusProperty);
    }

    public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
    {
        obj.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(TextBoxHelper));

    public static string GetWatermark(DependencyObject obj)
    {
        return (string)obj.GetValue(WatermarkProperty);
    }

    public static void SetWatermark(DependencyObject obj, string value)
    {
        obj.SetValue(WatermarkProperty, value);
    }

    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextBoxHelper));
}
