using CommunityToolkit.Mvvm.ComponentModel;
using MicaSetup.Controls;
using MicaSetup.Core;
using MicaSetup.Core.Helper;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MicaSetup.ViewModels;

public partial class InstallViewModel : ObservableObject
{
    [ObservableProperty]
    private string installInfo = string.Empty;

    [ObservableProperty]
    private double installProgress = 0d;

    public InstallViewModel()
    {
        Pack.Current.Installing = true;
        InstallInfo = Mui("Preparing");

        Task.Run(async () =>
        {
            await Task.Delay(200);

            try
            {
                using Stream archiveStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.7z");
                InstallInfo = Mui("ProgressTipsInstalling");
                InstallHelper.Install(archiveStream, (progress, key) =>
                {
                    UIDispatcherHelper.BeginInvoke(() =>
                    {
                        InstallProgress = progress * 100d;

                        if (Pack.Current.ShowInstallingFileName)
                        {
                            InstallInfo = key;
                        }
                    });
                });

                using Stream uninstStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/Uninst.exe");
                InstallHelper.CreateUninst(uninstStream);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            if (Pack.Current.AllowFullFolderSecurity)
            {
                try
                {
                    SecurityControlHelper.AllowFullFolderSecurity(Pack.Current.InstallLocation);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            InstallInfo = Mui("InstallFinishTips");
            Pack.Current.Installing = false;
            await Task.Delay(200);

            UIDispatcherHelper.Invoke(Routing.GoToNext);
        });
    }
}
