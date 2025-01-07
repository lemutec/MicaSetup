using MicaSetup.Attributes;
using MicaSetup.Natives;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MicaSetup.Helper;

[Auth(Auth.Admin)]
public static class StartMenuHelper
{
    public static void AddToRecent(string targetPath)
    {
        Shell32.SHAddToRecentDocs(SHARD.SHARD_PATHW, targetPath);
        if (Marshal.GetLastWin32Error() != 0)
        {
            throw new Win32Exception(nameof(Shell32.SHAddToRecentDocs));
        }
    }

    public static void CreateStartMenuFolder(string folderName, string targetPath, bool isCreateUninst = true)
    {
        string startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\Windows\Start Menu\Programs");
        string startMenuFolderPath = Path.Combine(startMenuPath, folderName);

        if (!Directory.Exists(startMenuFolderPath))
        {
            Directory.CreateDirectory(startMenuFolderPath);
        }
        ShortcutHelper.CreateShortcut(startMenuFolderPath, folderName, targetPath);
        AddToRecent(Path.Combine(startMenuFolderPath, $"{folderName}.lnk"));

        if (isCreateUninst)
        {
            string uninstTargetPath = Path.Combine($"{Path.GetDirectoryName(targetPath)}", "Uninst.exe");
            ShortcutHelper.CreateShortcut(startMenuFolderPath, $"Uninstall_{folderName}", uninstTargetPath);
        }
    }

    public static void RemoveStartMenuFolder(string folderName)
    {
        string startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\Windows\Start Menu\Programs");
        string startMenuFolderPath = Path.Combine(startMenuPath, folderName);

        if (Directory.Exists(startMenuFolderPath))
        {
            Directory.Delete(startMenuFolderPath, true);
        }
    }

    [Stable(false)]
    public static bool PinToStartMenu(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Logger.Warning("File does not exist.");
            return false;
        }

        try
        {
            Type shellType = Type.GetTypeFromProgID("Shell.Application");
            object shellObject = Activator.CreateInstance(shellType);
            dynamic folder = shellType.InvokeMember("Namespace", BindingFlags.InvokeMethod, null, shellObject, [Path.GetDirectoryName(filePath)]);
            dynamic item = folder.ParseName(Path.GetFileName(filePath));
            dynamic verbs = item.Verbs();

            foreach (dynamic verb in verbs)
            {
                string name = verb.Name.ToString();
                Logger.Information(verb.Name.ToString());

                if (name.EndsWith("(&P)", StringComparison.OrdinalIgnoreCase))
                {
                    verb.DoIt();
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        return false;
    }
}
