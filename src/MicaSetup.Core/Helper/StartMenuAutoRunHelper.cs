using Lnk;
using System;
using System.IO;

namespace MicaSetup.Core;

public static class StartMenuAutoRunHelper
{
    public static string StartupFolder => Environment.GetEnvironmentVariable("windir") + @"\..\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\";

    public static void Enable(string shortcutName, string targetPath, string arguments = null!)
    {
        try
        {
            if (Directory.Exists(StartupFolder))
            {
                ShortcutHelper.CreateShortcut(StartupFolder, shortcutName, targetPath, arguments);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
            NotificationHelper.AddNotice("Create Startup ShortCut error", "See detail following", e.ToString());
        }
    }

    public static bool IsEnabled(string shortcutName, string targetPath)
    {
        try
        {
            if (Directory.Exists(StartupFolder))
            {
                string lnk = StartupFolder + shortcutName + ".lnk";
                if (File.Exists(lnk))
                {
                    byte[] raw = File.ReadAllBytes(lnk);
                    if (raw[0] == 0x4c)
                    {
                        LnkFile lnkObj = new(raw, lnk);

                        if (lnkObj.LocalPath == targetPath)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
        return false;
    }

    public static void Disable(string shortcutName)
    {
        try
        {
            string lnk = StartupFolder + shortcutName + ".lnk";

            if (File.Exists(lnk))
            {
                File.Delete(lnk);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public static void SetEnabled(bool enable, string shortcutName, string targetPath, string arguments = null!)
    {
        if (enable)
        {
            Enable(shortcutName, targetPath, arguments);
        }
        else
        {
            Disable(shortcutName);
        }
    }
}
