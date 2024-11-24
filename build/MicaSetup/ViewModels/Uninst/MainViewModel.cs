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
    private bool isElevated = RuntimeHelper.IsElevated;

    [ObservableProperty]
    private bool keepMyData = Option.Current.KeepMyData;

    partial void OnKeepMyDataChanged(bool value)
    {
        Option.Current.KeepMyData = value;
        if (!value)
        {
            _ = Design.Controls.MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "NotKeepMyDataTips".Tr("KeepMyDataTips".Tr()));
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
            if (RuntimeHelper.IsElevated)
            {
                UninstallDataInfo uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath(Option.Current.KeyName);

                Option.Current.InstallLocation = uinfo.InstallLocation;
                if (!FileWritableHelper.CheckWritable(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName)))
                {
                    _ = Design.Controls.MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "LockedTipsAndExitTry".Tr(Option.Current.ExeName));
                    return;
                }
            }
            else
            {
                // Unable to check the filelock from register, so skip it.
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
        SystemCommands.CloseWindow(ApplicationDispatcherHelper.MainWindow);
    }
}

partial class MainViewModel
{
    public bool IsElevated
    {
        get => isElevated;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(isElevated, value))
            {
                OnIsElevatedChanging(value);
                OnIsElevatedChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("IsElevated"));
                isElevated = value;
                OnIsElevatedChanged(value);
                OnIsElevatedChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("IsElevated"));
            }
        }
    }

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

    partial void OnIsElevatedChanging(bool value);

    partial void OnIsElevatedChanging(bool oldValue, bool newValue);

    partial void OnIsElevatedChanged(bool value);

    partial void OnIsElevatedChanged(bool oldValue, bool newValue);

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
