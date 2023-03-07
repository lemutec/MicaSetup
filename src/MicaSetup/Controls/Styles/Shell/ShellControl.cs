using System;
using System.Windows;
using System.Windows.Controls;

namespace MicaSetup.Controls;

public class ShellControl : ContentControl
{
    public string Route
    {
        get => (string)GetValue(RouteProperty);
        set => SetCurrentValue(RouteProperty, value);
    }
    public static readonly DependencyProperty RouteProperty = DependencyProperty.Register("Route", typeof(string), typeof(ShellControl), new(string.Empty));

    public ShellControl()
    {
        Routing.Shell = new WeakReference<ShellControl>(this);
        Loaded += (_, _) => Routing.GoTo(Route);
    }
}
