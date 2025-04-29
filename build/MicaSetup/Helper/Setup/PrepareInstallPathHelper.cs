using MicaSetup.Attributes;
using System;

namespace MicaSetup.Helper;

[Auth(Auth.Admin | Auth.User)]
public static class PrepareInstallPathHelper
{
    public static string GetPrepareInstallPath(string keyName, bool preferX86 = false, bool preferAppDataLocalPrograms = false, bool preferAppDataRoaming = false)
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
            else if (preferAppDataLocalPrograms)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\" + Option.Current.KeyName;
            }
            else if (preferAppDataRoaming)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Option.Current.KeyName;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\" + Option.Current.KeyName;
        }
        else
        {
            if (preferAppDataLocalPrograms)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Programs\" + Option.Current.KeyName;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Option.Current.KeyName;
        }
    }
}
