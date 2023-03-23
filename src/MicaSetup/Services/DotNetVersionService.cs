using MicaSetup.Helper;
using System;
using System.Net;

namespace MicaSetup.Services;

public class DotNetVersionService : IDotNetVersionService
{
    public Version GetNetFrameworkVersion()
    {
        Version? version = DotNetVersionHelper.GetNet4xVersion();
        if (version != null)
        {
            return version;
        }

        version = DotNetVersionHelper.GetNet3xVersion();
        if (version != null)
        {
            return version;
        }

        version = DotNetVersionHelper.GetNet2xVersion();
        if (version != null)
        {
            return version;
        }

        version = DotNetVersionHelper.GetNet1xVersion();
        if (version != null)
        {
            return version;
        }
        return new Version();
    }

    public bool InstallNetFramework(Version version, DownloadProgressChangedEventHandler callback = null!)
    {
        throw new NotImplementedException();
    }
    
    public Version GetNetCoreVersion()
    {
        throw new NotImplementedException();
    }

    public bool InstallNetCore(Version version, DownloadProgressChangedEventHandler callback = null!)
    {
        throw new NotImplementedException();
    }
}
