using MicaSetup.Attributes;
using System;
using System.IO;
using System.Reflection;

namespace MicaSetup.Helper;

public static class PrepareUninstallPathHelper
{
    [Auth(Auth.Admin)]
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

    public static UninstallDataInfo GetPrepareUninstallPath()
    {
        try
        {
            string domainFilePath = string.Empty;

            if (!CommandLineHelper.Has(TempPathForkHelper.ForkedCli))
            {
                domainFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            }
            else
            {
                domainFilePath = CommandLineHelper.Values[TempPathForkHelper.ForkedCli];
            }

            string installLocation = Path.GetDirectoryName(domainFilePath);
            string uninstallDataPath = Path.Combine(installLocation, "Uninst.dat");
            string uninstallData = "Uninst.dat";

            if (File.Exists(uninstallDataPath))
            {
                uninstallData = $"{File.ReadAllText(uninstallDataPath).TrimEnd('|')}|{uninstallData}";
            }

            UninstallDataInfo uinfo = new()
            {
                InstallLocation = installLocation,
                UninstallData = uninstallData,
            };
            return uinfo;
        }
        catch
        {
        }
        return null!;
    }
}
