using MicaSetup.Win32;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace MicaSetup.Controls;

public class WindowTitleHeaderBehavior : Behavior<FrameworkElement>
{
    protected override void OnAttached()
    {
        AssociatedObject.Loaded += Loaded;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= Loaded;
        base.OnDetaching();
    }

    private void Loaded(object sender, EventArgs e)
    {
        AssociatedObject.RegisterAsTitleHeader();
    }
}

public static class RegisterAsTitleHeaderBehaviorExtension
{
    public static void RegisterAsTitleHeader(this UIElement self)
    {
        self.MouseLeftButtonDown += (s, e) =>
        {
            if (s is UIElement titleHeader && Window.GetWindow(titleHeader) is Window window)
            {
                if (e.ClickCount == 2)
                {
                    if (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip)
                    {
                        switch (window.WindowState)
                        {
                            case WindowState.Normal:
                                SystemCommands.MaximizeWindow(window);
                                break;
                            case WindowState.Maximized:
                                SystemCommands.RestoreWindow(window);
                                break;
                        }
                    }
                }
                else
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        User32.PostMessage(new WindowInteropHelper(window).Handle, (int)WindowMessage.WM_NCLBUTTONDOWN, (int)HitTestValues.HTCAPTION, 0);
                    }
                }
            }
        };

        self.MouseRightButtonDown += (s, e) =>
        {
            if (s is UIElement titleHeader && Window.GetWindow(titleHeader) is Window window)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (User32.GetCursorPos(out POINT pt))
                    {
                        SystemCommands.ShowSystemMenu(window, new Point(pt.X, pt.Y));
                    }
                }
            }
        };
    }
}
