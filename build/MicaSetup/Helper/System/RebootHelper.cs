using System;
using System.IO;

namespace MicaSetup.Helper;

public static class RebootHelper
{
    /// <summary>
    /// Restart the system immediately.
    /// </summary>
    /// <returns>True when the restart was initiated.</returns>
    public static bool Restart()
    {
        try
        {
            using FluentProcess process = FluentProcess.Create()
                .FileName(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shutdown.exe"))
                .Arguments("/r /t 0 /d p:4:2")
                .UseShellExecute(false)
                .CreateNoWindow(true)
                .Start()
                .WaitForExit();

            Logger.Information($"[RebootHelper] shutdown.exe exited with code {process.ExitCode}");
            return process.ExitCode == 0;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }
    }
}
