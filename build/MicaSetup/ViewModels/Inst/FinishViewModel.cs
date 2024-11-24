using MicaSetup.Design.Commands;
using MicaSetup.Design.ComponentModel;
using MicaSetup.Helper;
using System;
using System.IO;
using System.Windows;

namespace MicaSetup.ViewModels;

public partial class FinishViewModel : ObservableObject
{
    public string Message => Option.Current.MessageOfPage3;

    public FinishViewModel()
    {
    }

    [RelayCommand]
    public void Close()
    {
        if (ApplicationDispatcherHelper.MainWindow is Window window)
        {
            SystemCommands.CloseWindow(window);
        }
    }

    [RelayCommand]
    public void Open()
    {
        if (ApplicationDispatcherHelper.MainWindow is Window window)
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
            SystemCommands.CloseWindow(window);
        }
    }
}

partial class FinishViewModel
{
    private RelayCommand? closeCommand;
    public IRelayCommand CloseCommand => closeCommand ??= new RelayCommand(Close);

    private RelayCommand? openCommand;
    public IRelayCommand OpenCommand => openCommand ??= new RelayCommand(Open);
}
