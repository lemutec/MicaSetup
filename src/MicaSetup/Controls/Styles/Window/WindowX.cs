using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Win32;
using System.Windows;

namespace MicaSetup.Controls;

[INotifyPropertyChanged]
public partial class WindowX : Window
{
    public WindowX()
    {
        SystemCommands.CloseWindow(this);
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
        if (this.WindowState == WindowState.Maximized)
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
