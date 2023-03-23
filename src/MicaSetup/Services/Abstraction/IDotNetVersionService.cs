using System;
using System.Net;

namespace MicaSetup.Services;

public interface IDotNetVersionService
{
    public Version GetNetFrameworkVersion();
    public bool InstallNetFramework(Version version, DownloadProgressChangedEventHandler callback = null!);
    public Version GetNetCoreVersion();
    public bool InstallNetCore(Version version, DownloadProgressChangedEventHandler callback = null!);
}
