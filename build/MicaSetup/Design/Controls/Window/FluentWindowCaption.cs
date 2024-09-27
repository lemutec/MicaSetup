using System.Windows;
using System.Windows.Media;

namespace MicaSetup.Design.Controls;

public sealed class FluentWindowCaption
{
    public static ImageSource GetIcon(DependencyObject obj)
    {
        return (ImageSource)obj.GetValue(IconProperty);
    }

    public static void SetIcon(DependencyObject obj, ImageSource value)
    {
        obj.SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(FluentWindowCaption));

    public static Thickness GetPadding(DependencyObject obj)
    {
        return (Thickness)obj.GetValue(PaddingProperty);
    }

    public static void SetPadding(DependencyObject obj, Thickness value)
    {
        obj.SetValue(PaddingProperty, value);
    }

    public static readonly DependencyProperty PaddingProperty =
        DependencyProperty.RegisterAttached("Padding", typeof(Thickness), typeof(FluentWindowCaption));

    public static double GetHeight(DependencyObject obj)
    {
        return (double)obj.GetValue(HeightProperty);
    }

    public static void SetHeight(DependencyObject obj, double value)
    {
        obj.SetValue(HeightProperty, value);
    }

    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.RegisterAttached("Height", typeof(double), typeof(FluentWindowCaption));

    public static Brush GetForeground(DependencyObject obj)
    {
        return (Brush)obj.GetValue(ForegroundProperty);
    }

    public static void SetForeground(DependencyObject obj, Brush value)
    {
        obj.SetValue(ForegroundProperty, value);
    }

    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(FluentWindowCaption));

    public static Brush GetBackground(DependencyObject obj)
    {
        return (Brush)obj.GetValue(BackgroundProperty);
    }

    public static void SetBackground(DependencyObject obj, Brush value)
    {
        obj.SetValue(BackgroundProperty, value);
    }

    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(FluentWindowCaption));

    public static Style GetMinimizeButtonStyle(DependencyObject obj)
    {
        return (Style)obj.GetValue(MinimizeButtonStyleProperty);
    }

    public static void SetMinimizeButtonStyle(DependencyObject obj, Style value)
    {
        obj.SetValue(MinimizeButtonStyleProperty, value);
    }

    public static readonly DependencyProperty MinimizeButtonStyleProperty =
        DependencyProperty.RegisterAttached("MinimizeButtonStyle", typeof(Style), typeof(FluentWindowCaption));

    public static Style GetMaximizeButtonStyle(DependencyObject obj)
    {
        return (Style)obj.GetValue(MaximizeButtonStyleProperty);
    }

    public static void SetMaximizeButtonStyle(DependencyObject obj, Style value)
    {
        obj.SetValue(MaximizeButtonStyleProperty, value);
    }

    public static readonly DependencyProperty MaximizeButtonStyleProperty =
        DependencyProperty.RegisterAttached("MaximizeButtonStyle", typeof(Style), typeof(FluentWindowCaption));

    public static Style GetCloseButtonStyle(DependencyObject obj)
    {
        return (Style)obj.GetValue(CloseButtonStyleProperty);
    }

    public static void SetCloseButtonStyle(DependencyObject obj, Style value)
    {
        obj.SetValue(CloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CloseButtonStyleProperty =
        DependencyProperty.RegisterAttached("CloseButtonStyle", typeof(Style), typeof(FluentWindowCaption));

    public static Style GetFullScreenButtonStyle(DependencyObject obj)
    {
        return (Style)obj.GetValue(FullScreenButtonStyleProperty);
    }

    public static void SetFullScreenButtonStyle(DependencyObject obj, Style value)
    {
        obj.SetValue(FullScreenButtonStyleProperty, value);
    }

    public static readonly DependencyProperty FullScreenButtonStyleProperty =
        DependencyProperty.RegisterAttached("FullScreenButtonStyle", typeof(Style), typeof(FluentWindowCaption));

    public static object GetHeader(DependencyObject obj)
    {
        return obj.GetValue(HeaderProperty);
    }

    public static void SetHeader(DependencyObject obj, object value)
    {
        obj.SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.RegisterAttached("Header", typeof(object), typeof(FluentWindowCaption));

    public static UIElement GetExtendControl(DependencyObject obj)
    {
        return (UIElement)obj.GetValue(ExtendControlProperty);
    }

    public static void SetExtendControl(DependencyObject obj, UIElement value)
    {
        obj.SetValue(ExtendControlProperty, value);
    }

    public static readonly DependencyProperty ExtendControlProperty =
        DependencyProperty.RegisterAttached("ExtendControl", typeof(UIElement), typeof(FluentWindowCaption));

    public static bool GetDisableCloseButton(DependencyObject obj)
    {
        return (bool)obj.GetValue(DisableCloseButtonProperty);
    }

    public static void SetDisableCloseButton(DependencyObject obj, bool value)
    {
        obj.SetValue(DisableCloseButtonProperty, value);
    }

    public static readonly DependencyProperty DisableCloseButtonProperty =
        DependencyProperty.RegisterAttached("DisableCloseButton", typeof(bool), typeof(FluentWindowCaption));

    public static bool GetHideBasicButtons(DependencyObject obj)
    {
        return (bool)obj.GetValue(HideBasicButtonsProperty);
    }

    public static void SetHideBasicButtons(DependencyObject obj, bool value)
    {
        obj.SetValue(HideBasicButtonsProperty, value);
    }

    public static readonly DependencyProperty HideBasicButtonsProperty =
        DependencyProperty.RegisterAttached("HideBasicButtons", typeof(bool), typeof(FluentWindowCaption));

    public static bool GetShowFullScreenButton(DependencyObject obj)
    {
        return (bool)obj.GetValue(ShowFullScreenButtonProperty);
    }

    public static void SetShowFullScreenButton(DependencyObject obj, bool value)
    {
        obj.SetValue(ShowFullScreenButtonProperty, value);
    }

    public static readonly DependencyProperty ShowFullScreenButtonProperty =
        DependencyProperty.RegisterAttached("ShowFullScreenButton", typeof(bool), typeof(FluentWindowCaption), new(false));
}
