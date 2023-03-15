using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Controls;
using MicaSetup.Helper;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using MicaSetup.Shell.Dialogs;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace MicaSetup.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string installPath = PrepareInstallPathHelper.GetPrepareInstallPath(Option.Current.KeyName, Option.Current.UseInstallPathPreferX86);
    partial void OnInstallPathChanged(string value)
    {
        try
        {
            AvailableFreeSpace = DriveInfoHelper.GetAvailableFreeSpaceString(value);
            Option.Current.InstallLocation = value;
            IsIllegalPath = false;
        }
        catch
        {
            IsIllegalPath = true;
        }
    }

    [ObservableProperty]
    private string requestedFreeSpace = null!;

    [ObservableProperty]
    private string availableFreeSpace = null!;

    [ObservableProperty]
    private bool isIllegalPath = false;

    [ObservableProperty]
    private string licenseInfo = null!;

    [ObservableProperty]
    private bool licenseShown = false;

    [ObservableProperty]
    private bool licenseRead = true;
    partial void OnLicenseReadChanged(bool value)
    {
        CanStart = value;
    }

    [ObservableProperty]
    private bool canStart = true;

    [ObservableProperty]
    private bool isElevated = RuntimeHelper.IsElevated;

    public MainViewModel()
    {
        LicenseInfo = ResourceHelper.GetString($"pack://application:,,,/MicaSetup;component/Resources/Licenses/license.{GetLanguage()}.txt");
        using Stream archiveStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.7z");
        RequestedFreeSpace = ArchiveFileHelper.TotalUncompressSizeString(archiveStream);
        AvailableFreeSpace = DriveInfoHelper.GetAvailableFreeSpaceString(installPath);
    }

    [ObservableProperty]
    private bool installPathVisabale = false;

    [RelayCommand]
    private void ShowOrHideInstallPath(ToggleButton button)
    {
        button.IsEnabled = false;
        if (UIDispatcherHelper.MainWindow is Window window)
        {
            if (button.IsChecked ?? false)
            {
                DoubleAnimation animation = new()
                {
                    From = 400,
                    To = 520,
                    Duration = new Duration(TimeSpan.FromSeconds(0.15d)),
                    FillBehavior = FillBehavior.Stop,
                };
                animation.Completed += async (_, _) =>
                {
                    InstallPathVisabale = true;
                    await Task.Delay(50);
                    button.IsEnabled = true;
                };
                window.BeginAnimation(Window.HeightProperty, animation);
            }
            else
            {
                InstallPathVisabale = false;
                DoubleAnimation animation = new()
                {
                    From = 520,
                    To = 400,
                    Duration = new Duration(TimeSpan.FromSeconds(0.15d)),
                    FillBehavior = FillBehavior.Stop,
                };
                animation.Completed += async (_, _) =>
                {
                    await Task.Delay(50);
                    button.IsEnabled = true;
                };
                window.BeginAnimation(Window.HeightProperty, animation);
            }
        }
    }

    [RelayCommand]
    private void ShowOrHideLincenseInfo()
    {
        LicenseShown = !LicenseShown;
    }

    [RelayCommand]
    private void SelectFolder()
    {
        if (Option.Current.UseFolderPickerPreferClassic)
        {
            using FolderBrowserDialog dialog = new()
            {
                ShowNewFolderButton = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = dialog.SelectedPath;
                Option.Current.InstallLocation = InstallPath = selectedFolder;
            }
        }
        else
        {
            using CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedFolder = dialog.FileName;
                Option.Current.InstallLocation = InstallPath = selectedFolder;
            }
        }
    }

    [RelayCommand]
    private void StartInstall()
    {
        OnInstallPathChanged(InstallPath);

        if (IsIllegalPath)
        {
            _ = MessageBoxX.Info(UIDispatcherHelper.MainWindow, Mui("IllegalPathTips"));
            return;
        }

        if (UIDispatcherHelper.MainWindow is Window window)
        {
            window.Height = 400;
        }
        Routing.GoToNext();
    }
}
