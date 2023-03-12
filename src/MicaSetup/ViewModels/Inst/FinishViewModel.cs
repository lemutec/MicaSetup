using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Helper;
using System;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Reliability", "CA2000:")]
    public void Finish(Button button)
    {
        if (Window.GetWindow(button) is Window window)
        {
            if (AutoRunOnClosed)
            {
                try
                {
                    FluentProcess.Create()
                        .FileName(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName))
                        .WorkingDirectory(Option.Current.InstallLocation)
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
