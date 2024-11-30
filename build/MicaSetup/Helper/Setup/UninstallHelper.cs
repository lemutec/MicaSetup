using MicaSetup.Natives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MicaSetup.Helper;

public static class UninstallHelper
{
    public static void Uninstall(Action<double, string> progressCallback = null!, Action<UninstallReport, object> reportCallback = null!)
    {
        List<string> deleteDelayUntilRebootList = [];
        UninstallDataInfo uinfo = null!;

        if (RuntimeHelper.IsElevated)
        {
            uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath(Option.Current.KeyName);

            if (string.IsNullOrEmpty(uinfo.InstallLocation))
            {
                uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath();

                if (string.IsNullOrWhiteSpace(uinfo.UninstallData))
                {
                    MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "InstallationInfoLostHint".Tr());
                }
            }
        }
        else
        {
            // For security reason, uninst should always keep data because of unundering admin.
            Option.Current.KeepMyData = true;

            uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath();

            if (string.IsNullOrWhiteSpace(uinfo.UninstallData))
            {
                MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "InstallationInfoLostHint".Tr());
            }
        }

        Option.Current.InstallLocation = uinfo.InstallLocation;
        if (Option.Current.KeepMyData)
        {
            if (Directory.Exists(Option.Current.InstallLocation))
            {
                string[] uninstallDatas = uinfo.UninstallDataFullPath;
                double countMax = uninstallDatas.Length;
                int count = 0;
                List<string> dirs = [];

                foreach (string file in uninstallDatas)
                {
                    try
                    {
                        if ((File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            dirs.Add(file);
                        }
                        else
                        {
                            if (File.Exists(file))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        if (DeleteDelayUntilReboot(file))
                        {
                            deleteDelayUntilRebootList.Add(file);
                        }
                    }
                    progressCallback?.Invoke(Math.Min(++count / countMax, 1d), file);
                }

                if (Option.Current.IsCreateUninst)
                {
                    DeleteUninst();
                }

                foreach (string dir in dirs)
                {
                    try
                    {
                        if (Directory.Exists(dir))
                        {
                            DeleteEmptyDirectories(dir);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }

                try
                {
                    Directory.Delete(Option.Current.InstallLocation);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            else
            {
                progressCallback?.Invoke(1d, string.Empty);
            }
        }
        else
        {
            static int GetFileCount(string path)
            {
                int count = 0;
                count += Directory.GetFiles(path).Length;
                foreach (string subDir in Directory.GetDirectories(path))
                {
                    count += GetFileCount(subDir) + 1;
                }
                return count;
            }

            if (Directory.Exists(Option.Current.InstallLocation))
            {
                double countMax = GetFileCount(Option.Current.InstallLocation);
                int count = 0;

                foreach (string dir in Directory.GetDirectories(Option.Current.InstallLocation).Concat(new[] { Option.Current.InstallLocation }).ToList())
                {
                    foreach (string file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                            if (DeleteDelayUntilReboot(file))
                            {
                                deleteDelayUntilRebootList.Add(file);
                            }
                        }
                        progressCallback?.Invoke(Math.Min(++count / countMax, 1d), file);
                    }

                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        if (DeleteDelayUntilReboot(dir))
                        {
                            deleteDelayUntilRebootList.Add(dir);
                        }
                    }
                    progressCallback?.Invoke(Math.Min(++count / countMax, 1d), dir);
                }

                try
                {
                    Directory.Delete(Option.Current.InstallLocation);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    if (DeleteDelayUntilReboot(Option.Current.InstallLocation))
                    {
                        deleteDelayUntilRebootList.Add(Option.Current.InstallLocation);
                    }
                }
            }
            else
            {
                progressCallback?.Invoke(1d, string.Empty);
            }
        }

        if (Option.Current.IsInstallCertificate)
        {
            try
            {
                byte[] cer = ResourceHelper.GetBytes("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.cer");
                CertificateHelper.Uninstall(cer);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateDesktopShortcut)
        {
            try
            {
                ShortcutHelper.RemoveShortcutOnDesktop(Option.Current.DisplayName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateQuickLaunch)
        {
            try
            {
                ShortcutHelper.RemoveShortcutOnQuickLaunch(Option.Current.DisplayName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateStartMenu)
        {
            try
            {
                StartMenuHelper.RemoveStartMenuFolder(Option.Current.DisplayName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateAsAutoRun)
        {
            try
            {
                RegistyAutoRunHelper.Disable(Option.Current.KeyName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsEnvironmentVariable)
        {
            try
            {
                RemoveEnvironmentVariable(Option.Current.InstallLocation);

                foreach (string directory in Directory.EnumerateDirectories(Option.Current.InstallLocation, "*", SearchOption.AllDirectories))
                {
                    if (new DirectoryInfo(directory).Name == "bin")
                    {
                        RemoveEnvironmentVariable(directory);
                    }
                }

                static void RemoveEnvironmentVariable(string directoryPath)
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        EnvironmentVariableHelper.RemoveDirectoryFromUserPath(directoryPath);
                        EnvironmentVariableHelper.RemoveDirectoryFromSystemPath(directoryPath);
                    }
                    else
                    {
                        EnvironmentVariableHelper.RemoveDirectoryFromUserPath(directoryPath);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        try
        {
            RegistyUninstallHelper.Delete(Option.Current.KeyName);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        if (deleteDelayUntilRebootList.Any())
        {
            foreach (string deleteDelayUntilRebootItem in deleteDelayUntilRebootList.ConvertAll(p => p))
            {
                if (!Directory.Exists(deleteDelayUntilRebootItem) && !File.Exists(deleteDelayUntilRebootItem))
                {
                    deleteDelayUntilRebootList.Remove(deleteDelayUntilRebootItem);
                }
            }

            if (deleteDelayUntilRebootList.Any())
            {
                reportCallback?.Invoke(UninstallReport.AnyDeleteDelayUntilReboot, deleteDelayUntilRebootList.ToArray());
            }
        }
    }

    public static void DeleteUninst()
    {
        if (Option.Current.IsCreateUninst)
        {
            try
            {
                string uninstPath = Path.Combine(Option.Current.InstallLocation, "Uninst.exe");
                if (File.Exists(uninstPath))
                {
                    File.Delete(uninstPath);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    public static void DeleteEmptyDirectories(string rootDir)
    {
        string[] subDir = Directory.GetDirectories(rootDir);

        foreach (string dir in subDir)
        {
            DeleteEmptyDirectories(dir);
        }

        if (!Directory.EnumerateFileSystemEntries(rootDir).Any())
        {
            Directory.Delete(rootDir);
        }
    }

    /// <summary>
    /// <returns>
    /// True => File marked for deletion on next system startup.
    /// False => Failed to mark file for deletion on next system startup.
    /// </returns>
    public static bool DeleteDelayUntilReboot(string filePath)
    {
        if (Option.Current.IsEnableUninstallDelayUntilReboot)
        {
            return Kernel32.MoveFileEx(filePath, null!, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
        }
        else
        {
            // I had to pretend that the deletion was successful.
            return true;
        }
    }
}

public enum UninstallReport
{
    AnyDeleteDelayUntilReboot,
}
