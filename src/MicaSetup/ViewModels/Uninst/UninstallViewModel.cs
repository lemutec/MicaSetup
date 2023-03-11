﻿using CommunityToolkit.Mvvm.ComponentModel;
using MicaSetup.Controls;
using MicaSetup.Core;
using System.Threading.Tasks;

namespace MicaSetup.ViewModels;

public partial class UninstallViewModel : ObservableObject
{
    [ObservableProperty]
    private string installInfo = string.Empty;

    [ObservableProperty]
    private double installProgress = 0d;

    public UninstallViewModel()
    {
        Option.Current.Uninstalling = true;
        InstallInfo = Mui("Preparing");

        Task.Run(async () =>
        {
            await Task.Delay(200);
            InstallInfo = Mui("ProgressTipsUninstalling");

            UninstallHelper.Uninstall((progress, key) =>
            {
                UIDispatcherHelper.BeginInvoke(() =>
                {
                    InstallProgress = progress * 100d;
                    InstallInfo = key;
                });
            }, (report, _) =>
            {
                if (report == UninstallReport.AnyDeleteDelayUntilReboot)
                {
                    UIDispatcherHelper.Invoke(main =>
                    {
                        _ = MessageBoxX.Info(main, Mui("UninstallDelayUntilRebootTips"));
                    });
                }
            });

            Option.Current.Uninstalling = false;
            await Task.Delay(200);

            UIDispatcherHelper.Invoke(Routing.GoToNext);
        }).Forget();
    }
}
