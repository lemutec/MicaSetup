using PureSharpCompress.Common;
using PureSharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;

namespace MicaSetup.Helper;

public static class InstallHelper
{
    public static void Install(Stream archiveStream, Action<double, string> progressCallback = null!)
    {
        if (Option.Current.CloseApplications.Length > 0)
        {
            CloseApplicationHelper.CloseApplications();
        }

        if (Option.Current.IsInstallCertificate)
        {
            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    byte[] cer = ResourceHelper.GetBytes("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.cer");
                    CertificateHelper.Install(cer);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateDesktopShortcut)
        {
            try
            {
                ShortcutHelper.CreateShortcutOnDesktop(Option.Current.DisplayName, Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (!Directory.Exists(Option.Current.InstallLocation))
        {
            _ = Directory.CreateDirectory(Option.Current.InstallLocation);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(Option.Current.OverlayInstallRemoveExt))
            {
                HashSet<string> extFilters = new(StringComparer.OrdinalIgnoreCase);

                foreach (string extFilter in Option.Current.OverlayInstallRemoveExt.Split(','))
                {
                    string ext = extFilter.Trim();
                    if (string.IsNullOrEmpty(ext))
                    {
                        continue;
                    }

                    if (ext.StartsWith("."))
                    {
                        ext = ext.Substring(1);
                    }

                    if (!string.IsNullOrEmpty(ext))
                    {
                        _ = extFilters.Add(ext);
                    }
                }

                foreach (string file in Directory.GetFiles(Option.Current.InstallLocation, "*", SearchOption.AllDirectories))
                {
                    FileInfo fileInfo = new(file);
                    string fileExt = fileInfo.Extension.TrimStart('.');

                    if (string.IsNullOrEmpty(fileExt) || !extFilters.Contains(fileExt))
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            }

            if (Option.Current.OverlayInstallRemoveHandler != null
                && !Option.Current.OverlayInstallRemoveHandler.IsEmpty)
            {
                foreach (string file in Directory.GetFiles(Option.Current.InstallLocation, "*", SearchOption.AllDirectories))
                {
                    FileInfo fileInfo = new(file);

                    try
                    {
                        bool toRemove = Option.Current.OverlayInstallRemoveHandler.ToRemove(fileInfo);

                        if (toRemove)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            }
        }

        ReaderOptions readerOptions = new()
        {
            LookForHeader = true,
            Password = string.IsNullOrEmpty(Option.Current.UnpackingPassword) ? null! : Option.Current.UnpackingPassword,
        };

        ExtractionOptions extractionOptions = new()
        {
            ExtractFullPath = true,
            Overwrite = true,
            PreserveAttributes = false,
            PreserveFileTime = true,
        };

        HashSet<string> uninstallData = [];
#pragma warning disable IDE0350 // Use implicitly typed lambda
        ArchiveFileHelper.ExtractAll(Option.Current.InstallLocation, archiveStream, (double progress, string key) =>
        {
            Logger.Debug($"[ExtractAll] {key} {progress * 100d:0.00}%");
            progressCallback?.Invoke(progress, key);
            uninstallData.Add(key);
        }, readerOptions: readerOptions, options: extractionOptions);
#pragma warning restore IDE0350 // Use implicitly typed lambda

        if (Option.Current.IsCreateRegistryKeys && RuntimeHelper.IsElevated)
        {
            UninstallInfo info = new()
            {
                KeyName = Option.Current.KeyName,
                DisplayName = Option.Current.DisplayName,
                DisplayVersion = Option.Current.DisplayVersion,
                InstallLocation = Option.Current.InstallLocation,
                Publisher = Option.Current.Publisher,
                UninstallString = Option.Current.UninstallString,
                SystemComponent = Option.Current.SystemComponent,
            };

            if (string.IsNullOrWhiteSpace(Option.Current.DisplayIcon))
            {
                info.DisplayIcon = Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName);
            }
            else
            {
                info.DisplayIcon = Path.Combine(Option.Current.InstallLocation, Option.Current.DisplayIcon);
            }
            info.UninstallString ??= Path.Combine(Option.Current.InstallLocation, "Uninst.exe");
            info.UninstallData = string.Join("|", uninstallData); // The  '|' is a good split char for file path.

            try
            {
                RegistyUninstallHelper.Write(info);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        else
        {
            try
            {
                string uninstDataPath = Path.Combine(Option.Current.InstallLocation, "Uninst.dat");

                if (File.Exists(uninstDataPath))
                {
                    // Not allow user file named the same as it.
                    File.Delete(uninstDataPath);
                }
                File.WriteAllText(uninstDataPath, string.Join("|", uninstallData)); // The  '|' is a good split char for file path.
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateAsAutoRun)
        {
            try
            {
                RegistyAutoRunHelper.Enable(Option.Current.KeyName, $"\"{Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName)}\" {Option.Current.AutoRunLaunchCommand}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateQuickLaunch)
        {
            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    ShortcutHelper.CreateShortcutOnQuickLaunch(Option.Current.DisplayName, Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsPinToStartMenu)
        {
            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    _ = StartMenuHelper.PinToStartMenu(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsCreateStartMenu)
        {
            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    StartMenuHelper.CreateStartMenuFolder(Option.Current.DisplayName, Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName), Option.Current.IsCreateUninst);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        if (Option.Current.IsEnvironmentVariable)
        {
            try
            {
                AddEnvironmentVariable(Option.Current.InstallLocation);

                foreach (string directory in Directory.EnumerateDirectories(Option.Current.InstallLocation, "*", SearchOption.AllDirectories))
                {
                    if (new DirectoryInfo(directory).Name == "bin")
                    {
                        AddEnvironmentVariable(directory);
                    }
                }

                static void AddEnvironmentVariable(string directoryPath)
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        EnvironmentVariableHelper.AddDirectoryToUserPath(directoryPath);
                        EnvironmentVariableHelper.AddDirectoryToSystemPath(directoryPath);
                    }
                    else
                    {
                        EnvironmentVariableHelper.AddDirectoryToUserPath(directoryPath);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        try
        {
            StartMenuHelper.AddToRecent(Path.Combine(Option.Current.InstallLocation, Option.Current.ExeName));
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public static void CreateUninst(Stream uninstStream)
    {
        if (Option.Current.IsCreateUninst)
        {
            try
            {
                using FileStream fileStream = new(Path.Combine(Option.Current.InstallLocation, Option.Current.IsUninstLower ? "uninst.exe" : "Uninst.exe"), FileMode.Create);
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
