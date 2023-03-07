﻿using System;

namespace MicaSetup.Core;

public static class PrepareInstallPathHelper
{
    public static string GetPrepareInstallPath(string keyName, bool preferX86 = false)
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
        }
        if (preferX86)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\" + Pack.Current.KeyName;
        }
        return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\" + Pack.Current.KeyName;
    }
}
