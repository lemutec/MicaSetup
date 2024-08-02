using Microsoft.Win32;

namespace MakeMica.Cli.Core;

public static class CSharpScript
{
    private static string FindVSWhere()
    {
        string? uninstallInfo = (GetUninstallInfo(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", "Microsoft Visual Studio Installer")
                             ?? GetUninstallInfo(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", "Microsoft Visual Studio Installer"))
                             ?? throw new ApplicationException("Microsoft Visual Studio Installer is not installed, register not found.");

        string[] parsedArgs = ParseArguments(uninstallInfo);

        if (parsedArgs.Length <= 0)
        {
            throw new ApplicationException("Microsoft Visual Studio Installer is not installed, UninstallString is empty.");
        }

        FileInfo uninst = new(parsedArgs[0].Trim('"'));
        string drawio = Path.Combine(uninst.DirectoryName, "vswhere.exe");

        if (!File.Exists(drawio))
        {
            throw new ApplicationException("Microsoft Visual Studio Installer is not installed, file not found.");
        }

        return drawio;
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

    private static string[] ParseArguments(string commandLine)
    {
        List<string> args = [];
        string currentArg = string.Empty;
        bool inQuotes = false;

        for (int i = 0; i < commandLine.Length; i++)
        {
            char c = commandLine[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ' ' && !inQuotes)
            {
                if (currentArg != string.Empty)
                {
                    args.Add(currentArg);
                    currentArg = string.Empty;
                }
            }
            else
            {
                currentArg += c;
            }
        }

        if (currentArg != string.Empty)
        {
            args.Add(currentArg);
        }

        return [.. args];
    }
}
