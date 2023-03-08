using SharpCompress.Common;
using System;
using System.IO;
using System.Text;

namespace MicaSetup.Core.Helper;

public static class InstallHelper
{
    public static void Install(Stream archiveStream, Action<double, string> progressCallback = null!)
    {
        if (Pack.Current.DesktopShortcut)
        {
            try
            {
                ShortcutHelper.CreateShortcutOnDesktop(Pack.Current.DisplayName, Path.Combine(Pack.Current.InstallLocation, Pack.Current.ExeName));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (!Directory.Exists(Pack.Current.InstallLocation))
        {
            _ = Directory.CreateDirectory(Pack.Current.InstallLocation);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(Pack.Current.OverlayInstallRemoveExt))
            {
                string[] extFilters = Pack.Current.OverlayInstallRemoveExt.Split(',');

                foreach (string subDir in Directory.GetDirectories(Pack.Current.InstallLocation))
                {
                    foreach (string file in Directory.GetFiles(subDir, "*.*", SearchOption.AllDirectories))
                    {
                        FileInfo fileInfo = new(file);

                        foreach (string extFilter in extFilters)
                        {
                            string ext = extFilter;
                            if (ext.StartsWith("."))
                            {
                                ext = ext.Substring(1);
                            }
                            if (fileInfo.Extension.ToLower() == ext)
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (Exception e)
                                {
                                    Logger.Error(e);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        ExtractionOptions extractionOptions = new()
        {
            ExtractFullPath = true,
            Overwrite = true,
            PreserveAttributes = false,
            PreserveFileTime = true,
        };

        StringBuilder uninstallData = new();
        void progressCallback2(double progress, string key)
        {
            progressCallback?.Invoke(progress, key);
            uninstallData.Append(key);
            uninstallData.Append('|');
        }

        ArchiveFileHelper.ExtractAll(Pack.Current.InstallLocation, archiveStream, progressCallback2, options: extractionOptions);

        if (Pack.Current.RegistryKeys)
        {
            UninstallInfo info = new()
            {
                KeyName = Pack.Current.KeyName,
                DisplayName = Pack.Current.DisplayName,
                DisplayVersion = Pack.Current.DisplayVersion,
                InstallLocation = Pack.Current.InstallLocation,
                Publisher = Pack.Current.Publisher,
                UninstallString = Pack.Current.UninstallString,
                SystemComponent = Pack.Current.SystemComponent,
            };

            if (string.IsNullOrWhiteSpace(Pack.Current.DisplayIcon))
            {
                info.DisplayIcon = Path.Combine(Pack.Current.InstallLocation, Pack.Current.ExeName);
            }
            else
            {
                info.DisplayIcon = Path.Combine(Pack.Current.InstallLocation, Pack.Current.DisplayIcon);
            }
            info.UninstallString ??= Path.Combine(Pack.Current.InstallLocation, "Uninst.exe");
            info.UninstallData = uninstallData.ToString();

            try
            {
                RegistyUninstallHelper.Write(info);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        try
        {
            RegistyAutoRunHelper.SetEnabled(Pack.Current.AutoRun, Pack.Current.KeyName, $"{Path.Combine(Pack.Current.InstallLocation, Pack.Current.ExeName)} {Pack.Current.AutoRunLaunchCommand}");
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public static void CreateUninst(Stream uninstStream)
    {
        if (Pack.Current.CreateUninst)
        {
            try
            {
                using FileStream fileStream = new(Path.Combine(Pack.Current.InstallLocation, "Uninst.exe"), FileMode.Create);
                uninstStream.Seek(0, SeekOrigin.Begin);
                uninstStream.CopyTo(fileStream);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
