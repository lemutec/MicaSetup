using System.Runtime.InteropServices;
using System;
using System.IO;
using File = System.IO.File;

namespace MicaSetup.Helper;

public static class ShortcutHelper
{
    public static void CreateShortcut(string directory, string shortcutName, string targetPath, string arguments = null!, string description = null!, string iconLocation = null!)
    {
        if (!Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        string shortcutPath = Path.Combine(directory, $"{shortcutName}.lnk");

        dynamic shell = null!;
        dynamic shortcut = null!;

        try
        {
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            shell = Activator.CreateInstance(t);
            shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Arguments = arguments;
            shortcut.Description = description;
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;
            shortcut.Save();
        }
        finally
        {
            Marshal.FinalReleaseComObject(shortcut);
            Marshal.FinalReleaseComObject(shell);
        }
    }

    public static void CreateShortcutOnDesktop(string shortcutName, string targetPath, string arguments = null!, string description = null!, string iconLocation = null!)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        CreateShortcut(desktop, shortcutName, targetPath, arguments, description, iconLocation);
    }

    public static void RemoveShortcutOnDesktop(string shortcutName)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string filePath = Path.Combine(desktop, $"{shortcutName}.lnk");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
