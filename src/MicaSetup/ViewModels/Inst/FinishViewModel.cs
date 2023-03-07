using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MicaSetup.ViewModels;

public partial class FinishViewModel : ObservableObject
{
    [ObservableProperty]
    private bool autoRunOnClosed = true;

    public FinishViewModel()
    {
    }

    [RelayCommand]
    public void Finish(Button button)
    {
        if (Window.GetWindow(button) is Window window)
        {
            if (AutoRunOnClosed)
            {
                try
                {
                    FluentProcess.Create()
                        .FileName(Path.Combine(Pack.Current.InstallLocation, Pack.Current.ExeName))
                        .WorkingDirectory(Pack.Current.InstallLocation)
                        .UseShellExecute()
                        .Start()
                        .Forget();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            SystemCommands.CloseWindow(window);
        }
    }
}
