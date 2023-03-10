﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Win32;
using System;
using System.Windows;
using System.Windows.Interop;

namespace MicaSetup.Controls;

[INotifyPropertyChanged]
public partial class WindowX : Window
{
    public bool IsActivated
    {
        get => (bool)GetValue(IsActivatedProperty);
        set => SetValue(IsActivatedProperty, value);
    }
    public static readonly DependencyProperty IsActivatedProperty =  DependencyProperty.Register("IsActivated", typeof(bool), typeof(WindowX), new PropertyMetadata(false));

    public WindowX()
    {
        SystemCommands.CloseWindow(this);
    }

    protected override void OnActivated(EventArgs e)
    {
        IsActivated = true;
        base.OnActivated(e);
    }

    protected override void OnDeactivated(EventArgs e)
    {
        IsActivated = false;
        base.OnDeactivated(e);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        NativeMethods.HideAllWindowButton(new WindowInteropHelper(this).Handle);
    }

    [RelayCommand]
    private void CloseWindow()
    {
        SystemCommands.CloseWindow(this);
    }

    [RelayCommand]
    private void MinimizeWindow()
    {
        SystemCommands.MinimizeWindow(this);
    }

    [RelayCommand]
    private void MaximizeWindow()
    {
        SystemCommands.MaximizeWindow(this);
    }

    [RelayCommand]
    private void RestoreWindow()
    {
        SystemCommands.RestoreWindow(this);
    }

    [RelayCommand]
    private void MaximizeOrRestoreWindow()
    {
        if (WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(this);
        }
        else
        {
            SystemCommands.MaximizeWindow(this);
        }
    }

    [RelayCommand]
    private void ShowSystemMenu()
    {
        if (User32.GetCursorPos(out POINT pt))
        {
            SystemCommands.ShowSystemMenu(this, new Point(pt.X, pt.Y));
        }
    }
}
