using MicaSetup.Design.ComponentModel;
using MicaSetup.Design.Controls;
using MicaSetup.Helper;
using MicaSetup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace MicaSetup.ViewModels;

public partial class UninstallViewModel : ObservableObject
{
    public string Message => Option.Current.MessageOfPage2;

    [ObservableProperty]
    private string installInfo = string.Empty;

    [ObservableProperty]
    private double installProgress = 0d;

    public UninstallViewModel()
    {
        Option.Current.Uninstalling = true;
        InstallInfo = "Preparing".Tr();

        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            InstallInfo = "ProgressTipsUninstalling".Tr();

            UninstallHelper.Uninstall((progress, key) =>
            {
                ApplicationDispatcherHelper.BeginInvoke(() =>
                {
                    InstallProgress = progress * 100d;
                    InstallInfo = key;
                });
            }, (report, _) =>
            {
                if (report == UninstallReport.AnyDeleteDelayUntilReboot)
                {
                    ApplicationDispatcherHelper.Invoke(main =>
                    {
                        _ = MessageBox.Info(main, "UninstallDelayUntilRebootTips".Tr());
                    });
                }
            });

            if (Option.Current.IsAllowFirewall)
            {
                try
                {
                    FirewallHelper.RemoveApplication(Path.Combine(Option.Current.InstallLocation ?? string.Empty, Option.Current.ExeName));
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            Option.Current.Uninstalling = false;
            await Task.Delay(200);

            if (Option.Current.IsRefreshExplorer)
            {
                try
                {
                    ServiceManager.GetService<IExplorerService>()?.Refresh();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            ApplicationDispatcherHelper.Invoke(Routing.GoToNext);
        });
    }
}

partial class UninstallViewModel
{
    public string InstallInfo
    {
        get => installInfo;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(installInfo, value))
            {
                OnInstallInfoChanging(value);
                OnInstallInfoChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("InstallInfo"));
                installInfo = value;
                OnInstallInfoChanged(value);
                OnInstallInfoChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("InstallInfo"));
            }
        }
    }

    public double InstallProgress
    {
        get => installProgress;
        set
        {
            if (!EqualityComparer<double>.Default.Equals(installProgress, value))
            {
                OnInstallProgressChanging(value);
                OnInstallProgressChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("InstallProgress"));
                installProgress = value;
                OnInstallProgressChanged(value);
                OnInstallProgressChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("InstallProgress"));
            }
        }
    }

    partial void OnInstallInfoChanging(string value);

    partial void OnInstallInfoChanging(string? oldValue, string newValue);

    partial void OnInstallInfoChanged(string value);

    partial void OnInstallInfoChanged(string? oldValue, string newValue);

    partial void OnInstallProgressChanging(double value);

    partial void OnInstallProgressChanging(double oldValue, double newValue);

    partial void OnInstallProgressChanged(double value);

    partial void OnInstallProgressChanged(double oldValue, double newValue);
}
