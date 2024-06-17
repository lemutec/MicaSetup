﻿using MicaSetup.Attributes;

namespace MicaSetup.Helper;

[Auth(Auth.Admin)]
public static class PrepareUninstallPathHelper
{
    public static UninstallDataInfo GetPrepareUninstallPath(string keyName)
    {
        try
        {
            UninstallInfo info = RegistyUninstallHelper.Read(keyName);
            UninstallDataInfo uinfo = new()
            {
                InstallLocation = info.InstallLocation,
                UninstallData = info.UninstallData,
            };
            return uinfo;
        }
        catch
        {
        }
        return null!;
    }
}
