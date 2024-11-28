using Flucli.Utils.Extensions;
using Microsoft.Win32;

namespace MakeMica.Cli.Core;

public static class CSharpScript
{
    [Obsolete("Use MicaMacro.GetMicaDir instead")]
    public static string FindMicaDir()
    {
        string? uninstallInfo = (GetUninstallInfo(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", "MicaSetup")
                             ?? GetUninstallInfo(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", "MicaSetup"))
                             ?? throw new ApplicationException("MicaSetup is not installed, register not found.");

        FileInfo uninst = new(uninstallInfo.Trim('"'));
        string micadir = uninst.DirectoryName;

        if (!Directory.Exists(micadir))
        {
            throw new ApplicationException("MicaSetup is not installed, directory not found.");
        }

        return micadir;
    }

    public static string FindVSWhere()
    {
        string? uninstallInfo = (GetUninstallInfo(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", "Microsoft Visual Studio Installer")
                             ?? GetUninstallInfo(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", "Microsoft Visual Studio Installer"))
                             ?? throw new ApplicationException("Microsoft Visual Studio Installer is not installed, register not found.");

        string[] parsedArgs = [.. uninstallInfo.ToArguments()];

        if (parsedArgs.Length <= 0)
        {
            throw new ApplicationException("Microsoft Visual Studio Installer is not installed, UninstallString is empty.");
        }

        FileInfo uninst = new(parsedArgs[0].Trim('"'));
        string vswhere = Path.Combine(uninst.DirectoryName, "vswhere.exe");

        if (!File.Exists(vswhere))
        {
            throw new ApplicationException("Microsoft Visual Studio Installer is not installed, file not found.");
        }

        return vswhere;
    }

    private static string? GetUninstallInfo(string keyPath, string displayName)
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath);

        if (key != null)
        {
            foreach (string subkeyName in key.GetSubKeyNames())
            {
                using RegistryKey subkey = key.OpenSubKey(subkeyName);

                if (subkey != null)
                {
                    if (subkey.GetValue("DisplayName") is string name && name.Contains(displayName))
                    {
                        string? uninstallString = subkey.GetValue("UninstallString") as string;

                        if (!string.IsNullOrEmpty(uninstallString))
                        {
                            return uninstallString;
                        }
                    }
                }
            }
        }
        return null;
    }
}
