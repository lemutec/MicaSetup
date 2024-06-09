using MicaSetup.Design.Commands;
using MicaSetup.Design.ComponentModel;
using MicaSetup.Design.Controls;
using MicaSetup.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace MicaSetup.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public string Message => Option.Current.MessageOfPage1;

    [ObservableProperty]
    private bool keepMyData = Option.Current.KeepMyData;

    partial void OnKeepMyDataChanged(bool value)
    {
        Option.Current.KeepMyData = value;
        if (!value)
        {
            _ = MessageBoxX.Info(UIDispatcherHelper.MainWindow, Mui("NotKeepMyDataTips", Mui("KeepMyDataTips")));
        }
    }

    public MainViewModel()
    {
    }

    [RelayCommand]
    private void StartUninstall()
    {
        try
        {
            UninstallDataInfo uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath(Option.Current.KeyName);

            Option.Current.InstallLocation = uinfo.InstallLocation;
            if (!FileWritableHelper.CheckWritable(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName)))
            {
                _ = MessageBoxX.Info(UIDispatcherHelper.MainWindow, Mui("LockedTipsAndExitTry", Option.Current.ExeName));
                return;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        Routing.GoToNext();
    }

    [RelayCommand]
    public void CancelUninstall()
    {
        SystemCommands.CloseWindow(UIDispatcherHelper.MainWindow);
    }
}

partial class MainViewModel
{
    public bool KeepMyData
    {
        get => keepMyData;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(keepMyData, value))
            {
                OnKeepMyDataChanging(value);
                OnKeepMyDataChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("KeepMyData"));
                keepMyData = value;
                OnKeepMyDataChanged(value);
                OnKeepMyDataChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("KeepMyData"));
            }
        }
    }

    partial void OnKeepMyDataChanging(bool value);

    partial void OnKeepMyDataChanging(bool oldValue, bool newValue);

    partial void OnKeepMyDataChanged(bool value);

    partial void OnKeepMyDataChanged(bool oldValue, bool newValue);
}

partial class MainViewModel
{
    private RelayCommand? startUninstallCommand;

    public IRelayCommand StartUninstallCommand => startUninstallCommand ??= new RelayCommand(StartUninstall);

    private RelayCommand? cancelUninstallCommand;

    public IRelayCommand CancelUninstallCommand => cancelUninstallCommand ??= new RelayCommand(CancelUninstall);
}
