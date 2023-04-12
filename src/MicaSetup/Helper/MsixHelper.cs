using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MicaSetup.Helper;

public static class MsixHelper
{
    public async static Task<bool> Install(string msixPath, MsixInstallMethod method = default)
    {
        string latestAppInstallerPath = await FindLatestAppInstallerAsync();

        if (!string.IsNullOrEmpty(latestAppInstallerPath))
        {
            if (method == MsixInstallMethod.AppInstaller)
            {
                return await LaunchAppInstallerAsync(msixPath);
            }
            else if (method == MsixInstallMethod.AddAppxPackage)
            {
                return await AddAppxPackageAsync(msixPath);
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
    {
        return await Task.Run(() => LaunchAppInstaller(msixPath));
    }

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
    {
        return await Task.Run(FindLatestAppInstaller);
    }

    public static bool AddAppxPackage(string msixPath)
    {
        return FluentProcess.Create()
            .FileName("powershell.exe")
            .Arguments($"-Command \"Add-AppxPackage \"{msixPath}\"\"")
            .Start()
            .WaitForExit()
            .ExitCode == 0;
    }

    public async static Task<bool> AddAppxPackageAsync(string msixPath)
    {
        return await Task.Run(() => AddAppxPackage(msixPath));
    }
}

public enum MsixInstallMethod
{
    [Description("ms-appinstaller:")]
    AppInstaller,
    
    [Description("Add-AppxPackage")]
    AddAppxPackage,
}
