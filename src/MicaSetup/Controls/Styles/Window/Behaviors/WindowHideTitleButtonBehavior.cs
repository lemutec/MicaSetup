using MicaSetup.Win32;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Interop;

namespace MicaSetup.Controls;

public class WindowHideTitleButtonBehavior : Behavior<Window>
{
    protected override void OnAttached()
    {
        AssociatedObject.SourceInitialized += OnSourceInitialized;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnSourceInitialized;
        base.OnDetaching();
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
        NativeMethods.HideAllWindowButton(new WindowInteropHelper(AssociatedObject).Handle);
    }
}
