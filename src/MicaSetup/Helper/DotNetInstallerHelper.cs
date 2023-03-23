using System;
using System.ComponentModel;
using System.IO;

namespace MicaSetup.Helper;

#pragma warning disable IDE0060

/// <summary>
/// https://dotnet.microsoft.com/en-us/download/visual-studio-sdks
/// </summary>
public static class DotNetInstallerHelper
{
    public static DotNetInstallInfo GetInfo(Version version, bool offline = true)
    {
        DotNetInstallInfo info = new()
        {
            Offline = offline,
            Version = version,
        };

        if (version == new Version(4, 8, 1))
        {
            info.FileName = offline switch
            {
                true => "ndp481-x86-x64-allos-enu.exe",
                false => "ndp481-web.exe",
            };
            info.Arguments = " /q /norestart /ChainingPackage FullX64Bootstrapper";
            info.DownloadUrl = offline switch
            {
                true => "https://download.visualstudio.microsoft.com/download/pr/6f083c7e-bd40-44d4-9e3f-ffba71ec8b09/3951fd5af6098f2c7e8ff5c331a0679c/ndp481-x86-x64-allos-enu.exe",
                false => "https://download.visualstudio.microsoft.com/download/pr/6f083c7e-bd40-44d4-9e3f-ffba71ec8b09/d05099507287c103a91bb68994498bde/ndp481-web.exe",
            };
            info.ReleaseNoteUrl = "https://devblogs.microsoft.com/dotnet/announcing-dotnet-framework-481";
        }
        else if (version == new Version(4, 8, 0))
        {
            info.FileName = offline switch
            {
                true => "ndp48-x86-x64-allos-enu.exe",
                false => "ndp48-web.exe",
            };
            info.Arguments = " /q /norestart /ChainingPackage FullX64Bootstrapper";
            info.DownloadUrl = offline switch
            {
                true => "https://download.visualstudio.microsoft.com/download/pr/2d6bb6b2-226a-4baa-bdec-798822606ff1/8494001c276a4b96804cde7829c04d7f/ndp48-x86-x64-allos-enu.exe",
                false => "https://download.visualstudio.microsoft.com/download/pr/2d6bb6b2-226a-4baa-bdec-798822606ff1/9b7b8746971ed51a1770ae4293618187/ndp48-web.exe",
            };
            info.ReleaseNoteUrl = "https://devblogs.microsoft.com/dotnet/announcing-the-net-framework-4-8";
        }
        else
        {
            throw new NotImplementedException();
        }
        return info;
    }

    public static bool Download(DotNetInstallInfo info, Action<object, ProgressChangedEventArgs> callback = null!)
    {
        _ = info.Version ?? throw new NotImplementedException();
        _ = info.DownloadUrl ?? throw new NotImplementedException();
        _ = info.FileName ?? throw new NotImplementedException();

        string installerPath = Path.Combine(SpecialPathHelper.TempPath.SureDirectoryExists(), info.FileName);
        Logger.Info($"[DotNetInstaller] Download .NET Framework {info.Version} from '{info.DownloadUrl}' and save to '{installerPath}'.");

        return SimpleDownloadHelper.DownloadFile(info.DownloadUrl, installerPath, (s, e) => callback?.Invoke(s, e));
    }

    public static bool Install(DotNetInstallInfo info, Action<object, ProgressChangedEventArgs> callback = null!)
    {
        _ = info.Version ?? throw new NotImplementedException();
        _ = info.FileName ?? throw new NotImplementedException();
        _ = info.Arguments ?? throw new NotImplementedException();

        string installerPath = Path.Combine(SpecialPathHelper.TempPath.SureDirectoryExists(), info.FileName);
        Logger.Info($"[DotNetInstaller] Install .NET Framework {info.Version} from '{installerPath}'.");

        return FluentProcess.Create()
            .FileName(installerPath)
            .Arguments(info.Arguments)
            .Start()
            .WaitForExit()
            .ExitCode == 0;
    }
}

public class DotNetInstallInfo
{
    public Version Version { get; set; } = null!;
    public string DownloadUrl { get; set; } = null!;
    public string ReleaseNoteUrl { get; set; } = null!;
    public bool Offline { get; set; } = false;
    public string FileName { get; set; } = null!;
    public string Arguments { get; set; } = null!;
}
