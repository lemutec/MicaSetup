using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MicaSetup.Helper;

public static class MsixHelper
{
    public async static Task<bool> HasAppInstaller()
    {
        if (!OsHelper.IsWindows10_OrGreater)
        {
            return false;
        }

        if (IsLongTermServicingSystem())
        {
            Logger.Warn("[MsixHelper] Long-Term Servicing Channel/Branch detected. (LTSC/LTSB)");
        }

        string latestAppInstallerPath = await FindLatestAppInstallerAsync();
        return !string.IsNullOrEmpty(latestAppInstallerPath);
    }

    public async static Task<bool> Install(string msixPath, MsixInstallMethod method = default)
    {
        return method switch
        {
            MsixInstallMethod.AppInstaller => await LaunchAppInstallerAsync(msixPath),
            MsixInstallMethod.PowerShell => await AddAppxPackageAsync(msixPath),
            _ => false,
        };
    }

    public async static Task<bool> Uninstall()
    {
        string pfn = await GetPackageFullNameAsync(Option.Current.AppxPackageName ?? throw new ArgumentNullException(nameof(Option.Current.AppxPackageName)));

        if (string.IsNullOrEmpty(pfn))
        {
            return await RemoveAppxPackageAsync(pfn);
        }
        return false;
    }

    public static bool IsLongTermServicingSystem()
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        if (key != null)
        {
            string? productName = key.GetValue("ProductName") as string;

            if (!string.IsNullOrEmpty(productName))
            {
                // LTSC: Long-Term Servicing Channel
                // LTSB: Long-Term Servicing Branch
                return productName!.Contains("LTSC") || productName.Contains("LTSB");
            }
        }
        return false;
    }

    public static bool LaunchAppInstaller(string msixPath)
    {
        return FluentProcess.Start("ms-appinstaller:?source=" + Uri.EscapeDataString(msixPath))
            .WaitForExit()
            .ExitCode == 0;
    }

    public async static Task<bool> LaunchAppInstallerAsync(string msixPath)
        => await Task.Run(() => LaunchAppInstaller(msixPath));

    public static string FindLatestAppInstaller()
    {
        string searchRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WindowsApps");
        string[] appInstallerPaths = Directory.GetFiles(searchRoot, "AppInstaller.exe", SearchOption.AllDirectories);

        string latestAppInstallerPath = null!;
        Version latestVersion = new();

        foreach (string path in appInstallerPaths)
        {
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(path);

            if (fileInfo != null && !string.IsNullOrEmpty(fileInfo.FileVersion) && string.Equals(fileInfo.CompanyName, "Microsoft Corporation", StringComparison.OrdinalIgnoreCase))
            {
                Version version = new(fileInfo.FileVersion);

                if (version > latestVersion)
                {
                    latestVersion = version;
                    latestAppInstallerPath = path;
                }
            }
        }

        return latestAppInstallerPath;
    }

    public async static Task<string> FindLatestAppInstallerAsync()
        => await Task.Run(FindLatestAppInstaller);

    public static bool AddAppxPackage(string msixPath)
    {
        return FluentProcess.Create()
            .FileName("powershell.exe")
            .Arguments($"-Command \"Add-AppxPackage \"{msixPath}\"\"")
            .Verb("runas")
            .Start()
            .WaitForExit()
            .ExitCode == 0;
    }

    public async static Task<bool> AddAppxPackageAsync(string msixPath)
        => await Task.Run(() => AddAppxPackage(msixPath));

    public static bool RemoveAppxPackage(string msixPath)
    {
        return FluentProcess.Create()
            .FileName("powershell.exe")
            .Arguments($"-Command \"Remove-AppxPackage \"{msixPath}\"\"")
            .Verb("runas")
            .Start()
            .WaitForExit()
            .ExitCode == 0;
    }

    public async static Task<bool> RemoveAppxPackageAsync(string msixPath)
        => await Task.Run(() => RemoveAppxPackage(msixPath));

    public async static Task<string> GetPackageFullNameAsync(string packageName)
        => await Task.Run(() => GetAppxPackage(packageName, "PackageFullName"));

    public static string GetPackageFullName(string packageName)
        => GetAppxPackage(packageName, "PackageFullName");

    public static string GetAppxPackage(string packageName, string key = null!)
    {
        using MemoryStream stream = new();
        int exitCode = FluentProcess.Create()
            .FileName("powershell.exe")
            .Arguments($"-Command \"Get-AppxPackage -Name \"{packageName}\"\"")
            .Verb("runas")
            .CreateNoWindow()
            .UseShellExecute(false)
            .RedirectStandardOutput()
            .Start()
            .BeginOutputRead(stream)
            .WaitForExit()
            .ExitCode;

        if (exitCode != 0)
        {
            return null!;
        }

        string lines = Encoding.UTF8.GetString(stream.ToArray());

        if (lines != null)
        {
            foreach (string line in lines.Split('\n'))
            {
                if (line.Trim().StartsWith(key))
                {
                    string[] cs = line.Split(':');

                    if (cs.Length >= 2)
                    {
                        return cs[1].Trim();
                    }
                }
            }
        }

        return null!;
    }
}

public enum MsixInstallMethod
{
    AppInstaller,
    PowerShell,
}
