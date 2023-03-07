using MicaSetup.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MicaSetup.Core;

public static class UninstallHelper
{
    public static void Uninstall(Action<double, string> progressCallback = null!, Action<UninstallReport, object> reportCallback = null!)
    {
        bool anyDeleteDelayUntilReboot = false;
        List<string> deleteDelayUntilRebootList = new();
        UninstallDataInfo uinfo = PrepareUninstallPathHelper.GetPrepareUninstallPath(Pack.Current.KeyName);

        Pack.Current.InstallLocation = uinfo.InstallLocation;
        if (Pack.Current.KeepMyData)
        {
            if (Directory.Exists(Pack.Current.InstallLocation))
            {
                string[] uninstallDatas = uinfo.UninstallDataFullPath;
                double countMax = uninstallDatas.Length;
                int count = 0;
                List<string> dirs = new();

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
                            anyDeleteDelayUntilReboot = true;
                        }
                    }
                    progressCallback?.Invoke(Math.Min(++count / countMax, 1d), file);
                }

                if (Pack.Current.CreateUninst)
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
                    Directory.Delete(Pack.Current.InstallLocation);
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

            if (Directory.Exists(Pack.Current.InstallLocation))
            {
                double countMax = GetFileCount(Pack.Current.InstallLocation);
                int count = 0;

                foreach (string dir in Directory.GetDirectories(Pack.Current.InstallLocation).Concat(new[] { Pack.Current.InstallLocation }).ToList())
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
                                anyDeleteDelayUntilReboot = true;
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
                            anyDeleteDelayUntilReboot = true;
                        }
                    }
                    progressCallback?.Invoke(Math.Min(++count / countMax, 1d), dir);
                }

                try
                {
                    Directory.Delete(Pack.Current.InstallLocation);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    if (DeleteDelayUntilReboot(Pack.Current.InstallLocation))
                    {
                        deleteDelayUntilRebootList.Add(Pack.Current.InstallLocation);
                        anyDeleteDelayUntilReboot = true;
                    }
                }
            }
            else
            {
                progressCallback?.Invoke(1d, string.Empty);
            }
        }

        if (Pack.Current.DesktopShortcut)
        {
            try
            {
                ShortcutHelper.RemoveShortcutOnDesktop(Pack.Current.DisplayName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Pack.Current.AutoRun)
        {
            try
            {
                RegistyAutoRunHelper.Disable(Pack.Current.KeyName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        try
        {
            RegistyUninstallHelper.Delete(Pack.Current.KeyName);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        if (anyDeleteDelayUntilReboot)
        {
            reportCallback?.Invoke(UninstallReport.AnyDeleteDelayUntilReboot, deleteDelayUntilRebootList.ToArray());
        }
    }


    public static void DeleteUninst()
    {
        if (Pack.Current.CreateUninst)
        {
            try
            {
                string uninstPath = Path.Combine(Pack.Current.InstallLocation, "Uninst.exe");
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
        return Kernel32.MoveFileEx(filePath, null!, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
    }
}

public enum UninstallReport
{
    AnyDeleteDelayUntilReboot,
}
