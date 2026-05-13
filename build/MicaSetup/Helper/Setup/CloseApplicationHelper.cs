using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MicaSetup.Helper;

public static class CloseApplicationHelper
{
    /// <summary>
    /// Closes all applications listed in <see cref="Option.Current.CloseApplications"/>.
    /// Call this before starting install or uninstall to prevent locked-file warnings.
    /// Equivalent to WiX util:CloseApplication.
    /// </summary>
    public static void CloseApplications()
        => CloseApplications(Option.Current.CloseApplications);

    public static void CloseApplications(IEnumerable<CloseApplicationInfo> applications)
    {
        foreach (CloseApplicationInfo info in applications)
        {
            try
            {
                CloseApplication(info);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    private static void CloseApplication(CloseApplicationInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.Target))
        {
            return;
        }

        string processName = Path.GetFileNameWithoutExtension(info.Target);
        Process[] processes = Process.GetProcessesByName(processName);

        foreach (Process process in processes)
        {
            try
            {
                if (process.HasExited)
                {
                    continue;
                }

                if (info.CloseMessage)
                {
                    // Try a graceful close via WM_CLOSE first.
                    process.CloseMainWindow();

                    int timeoutMs = info.Timeout * 1000;
                    if (process.WaitForExit(timeoutMs))
                    {
                        continue;
                    }
                }

                if (info.TerminateProcess)
                {
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            finally
            {
                process.Dispose();
            }
        }
    }
}
