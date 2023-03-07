using IWshRuntimeLibrary;
using System;
using System.IO;
using File = System.IO.File;

namespace MicaSetup.Core;

public static class ShortcutHelper
{
    public static void CreateShortcut(string directory, string shortcutName, string targetPath, string arguments = null!, string description = null!, string iconLocation = null!)
    {
        if (!Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }
 
        string shortcutPath = Path.Combine(directory, $"{shortcutName}.lnk");
        WshShell shell = new();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = targetPath;
        shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
        shortcut.WindowStyle = 1;
        shortcut.Arguments = arguments;
        shortcut.Description = description;
        shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;
        shortcut.Save();
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
