using CommunityToolkit.Mvvm.ComponentModel;
using MicaSetup.Controls;
using MicaSetup.Helper;
using MicaSetup.Helper.Helper;
using MicaSetup.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace MicaSetup.ViewModels;

public partial class InstallViewModel : ObservableObject
{
    [ObservableProperty]
    private string installInfo = string.Empty;

    [ObservableProperty]
    private double installProgress = 0d;

    [SuppressMessage("Design", "CA1031:")]
    public InstallViewModel()
    {
        Option.Current.Installing = true;
        InstallInfo = Mui("Preparing");

        _ = Task.Run(async () =>
        {
            await Task.Delay(200).ConfigureAwait(true);

            try
            {
                using Stream archiveStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.7z");
                InstallInfo = Mui("ProgressTipsInstalling");
                InstallHelper.Install(archiveStream, (progress, key) =>
                {
                    UIDispatcherHelper.BeginInvoke(() =>
                    {
                        InstallProgress = progress * 100d;
                        InstallInfo = key;
                    });
                });

                using Stream uninstStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/Uninst.exe");
                InstallHelper.CreateUninst(uninstStream);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            if (Option.Current.IsAllowFullFolderSecurity)
            {
                try
                {
                    SecurityControlHelper.AllowFullFolderSecurity(Option.Current.InstallLocation);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            InstallInfo = Mui("InstallFinishTips");
            Option.Current.Installing = false;
            await Task.Delay(200).ConfigureAwait(false);

            ServiceProviderX.Current.GetService<IExplorerService>()!.Refresh();
            UIDispatcherHelper.Invoke(Routing.GoToNext);
        });
    }
}
