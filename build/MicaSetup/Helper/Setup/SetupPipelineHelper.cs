using MicaSetup.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

#if SETUP
using PureSharpCompress.Readers;
#endif

namespace MicaSetup.Helper;

/// <summary>
/// Install and uninstall steps without any UI interaction,
/// shared by the wizard pages and the unattended CLI (quiet / passive).
/// </summary>
public static class SetupPipelineHelper
{
#if SETUP
    public static string GetDefaultInstallLocation()
    {
        string installPath = PrepareInstallPathHelper.GetPrepareInstallPath(Option.Current.KeyName,
            Option.Current.IsUseInstallPathPreferX86,
            Option.Current.IsUseInstallPathPreferAppDataLocalPrograms,
            Option.Current.IsUseInstallPathPreferAppDataRoaming);

        return CliParser.NormalizeInstallLocation(installPath) ?? installPath;
    }

    /// <summary>
    /// Validate the install location like the wizard does before installing.
    /// </summary>
    public static bool PrepareInstall()
    {
        if (string.IsNullOrWhiteSpace(Option.Current.InstallLocation))
        {
            Option.Current.InstallLocation = GetDefaultInstallLocation();
        }
        Logger.Information($"[InstallLocation] {Option.Current.InstallLocation}");

        try
        {
            using Stream archiveStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.7z");
            ReaderOptions readerOptions = new()
            {
                LookForHeader = true,
                Password = string.IsNullOrEmpty(Option.Current.UnpackingPassword) ? null! : Option.Current.UnpackingPassword,
            };
            long requestedFreeSpace = ArchiveFileHelper.TotalUncompressSize(archiveStream, readerOptions) + (Option.Current.IsCreateUninst ? 2048000 : 0);
            long availableFreeSpace = DriveInfoHelper.GetAvailableFreeSpace(Option.Current.InstallLocation);

            if (requestedFreeSpace >= availableFreeSpace)
            {
                Logger.Error($"[PrepareInstall] Available free space is insufficient, requested {requestedFreeSpace.ToFreeSpaceString()} but available {availableFreeSpace.ToFreeSpaceString()}.");
                return false;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }

        try
        {
            if (!FileWritableHelper.CheckWritable(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName)))
            {
                Logger.Error($"[PrepareInstall] {Option.Current.ExeName} is locked.");
                return false;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
        return true;
    }

    public static bool Install(Action<double, string> progressCallback = null!)
    {
        bool success = true;

        try
        {
            using Stream archiveStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.7z");
            InstallHelper.Install(archiveStream, progressCallback);

            using Stream uninstStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/Uninst.exe");
            InstallHelper.CreateUninst(uninstStream);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            success = false;
        }

        if (Option.Current.IsAllowFullFolderSecurity)
        {
            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    SecurityControlHelper.AllowFullFolderSecurity(Option.Current.InstallLocation);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsAllowFirewall)
        {
            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    FirewallHelper.AllowApplication(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        return success;
    }
#endif

#if UNINST
    /// <summary>
    /// Resolve the install location and check the file lock like the wizard does before uninstalling.
    /// </summary>
    public static bool PrepareUninstall()
    {
        try
        {
            if (RuntimeHelper.IsElevated)
            {
                UninstallDataInfo uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath(Option.Current.KeyName);

                Option.Current.InstallLocation = uinfo.InstallLocation;
                if (!FileWritableHelper.CheckWritable(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName)))
                {
                    Logger.Error($"[PrepareUninstall] {Option.Current.ExeName} is locked.");
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
        return true;
    }

    public static bool Uninstall(Action<double, string> progressCallback = null!, Action<UninstallReport, object> reportCallback = null!)
    {
        bool success = true;

        try
        {
            UninstallHelper.Uninstall(progressCallback, (report, data) =>
            {
                if (report == UninstallReport.AnyDeleteDelayUntilReboot)
                {
                    Option.Current.RebootRequired = true;
                }
                reportCallback?.Invoke(report, data);
            });
        }
        catch (Exception e)
        {
            Logger.Error(e);
            success = false;
        }

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
        return success;
    }
#endif

    public static void RefreshExplorer()
    {
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
    }

    /// <summary>
    /// Run the whole pipeline for quiet mode on a background thread.
    /// </summary>
    public static Task<int> RunUnattendedAsync()
    {
        return Task.Run(() =>
        {
#if SETUP
            if (!PrepareInstall())
            {
                return CliExitCode.Failure;
            }

            Option.Current.Installing = true;
            bool success = Install();
            Option.Current.Installing = false;
#elif UNINST
            if (!PrepareUninstall())
            {
                return CliExitCode.Failure;
            }

            Option.Current.Uninstalling = true;
            bool success = Uninstall();
            Option.Current.Uninstalling = false;
#else
            bool success = false;
#endif
            RefreshExplorer();
            return Complete(success);
        });
    }

    /// <summary>
    /// Get the exit code and apply the restart policy like msiexec:
    /// `/forcerestart` always restarts after success, otherwise a required restart is reported by 3010.
    /// </summary>
    public static int Complete(bool success)
    {
        if (!success)
        {
            return CliExitCode.Failure;
        }

        if (Option.Current.CliForceRestart && RebootHelper.Restart())
        {
            return CliExitCode.RebootInitiated;
        }
        return Option.Current.RebootRequired ? CliExitCode.RebootRequired : CliExitCode.Success;
    }

    /// <summary>
    /// Shutdown the application with exit code, must be called on the UI thread.
    /// </summary>
    public static void Exit(int exitCode)
    {
        Logger.Information($"[CLI] Exit with code {exitCode}");
        try
        {
            TempPathForkHelper.Clean();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
        finally
        {
            Application.Current.Shutdown(exitCode);
        }
    }
}
