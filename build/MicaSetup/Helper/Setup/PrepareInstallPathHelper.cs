using MicaSetup.Attributes;
using System;

namespace MicaSetup.Helper;

[Auth(Auth.Admin | Auth.User)]
public static class PrepareInstallPathHelper
{
    public static string GetPrepareInstallPath(string keyName, bool preferX86 = false)
    {
        if (RuntimeHelper.IsElevated)
        {
            try
            {
                UninstallInfo info = RegistyUninstallHelper.Read(keyName);

                if (!string.IsNullOrWhiteSpace(info.InstallLocation))
                {
                    return info.InstallLocation;
                }
            }
            catch
            {
                ///
            }

            if (preferX86)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\" + Option.Current.KeyName;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\" + Option.Current.KeyName;
        }
        else
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Option.Current.KeyName;
        }
    }
}
