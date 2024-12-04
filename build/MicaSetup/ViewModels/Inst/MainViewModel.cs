using MicaSetup.Design.Commands;
using MicaSetup.Design.ComponentModel;
using MicaSetup.Design.Controls;
using MicaSetup.Helper;
using MicaSetup.Services;
using MicaSetup.Shell.Dialogs;
using PureSharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace MicaSetup.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public string Message => Option.Current.MessageOfPage1;

    [ObservableProperty]
    private string installPath = PrepareInstallPathHelper.GetPrepareInstallPath(Option.Current.KeyName, Option.Current.IsUseInstallPathPreferX86);

    partial void OnInstallPathChanged(string value)
    {
        try
        {
            value = Path.Combine(value).TrimEnd('\\', '/');
            if (value.EndsWith(":"))
            {
                value += Path.DirectorySeparatorChar;
            }
            availableFreeSpaceLong = DriveInfoHelper.GetAvailableFreeSpace(value);
            AvailableFreeSpace = availableFreeSpaceLong.ToFreeSpaceString();
            Option.Current.InstallLocation = (value?.EndsWith(Option.Current.KeyName, StringComparison.OrdinalIgnoreCase) ?? false) ? value : Path.Combine(value, Option.Current.KeyName);
            Logger.Debug($"[InstallLocation] {Option.Current.InstallLocation}");
            IsIllegalPath = false;
        }
        catch
        {
            IsIllegalPath = true;
        }
    }

    [ObservableProperty]
    private string requestedFreeSpace = null!;

    private long requestedFreeSpaceLong = default;

    [ObservableProperty]
    private string availableFreeSpace = null!;

    private long availableFreeSpaceLong = default;

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

    [ObservableProperty]
    private bool isCustomizeVisiableAutoRun = Option.Current.IsCustomizeVisiableAutoRun;

    [ObservableProperty]
    private bool autoRun = Option.Current.IsCreateAsAutoRun;

    partial void OnAutoRunChanged(bool value)
    {
        Option.Current.IsCreateAsAutoRun = value;
    }

    [ObservableProperty]
    private bool desktopShortcut = Option.Current.IsCreateDesktopShortcut;

    partial void OnDesktopShortcutChanged(bool value)
    {
        Option.Current.IsCreateDesktopShortcut = value;
    }

    public MainViewModel()
    {
        string? licenseUrl = ServiceManager.GetService<ITrService>()?.GetLicenseUriString();
        LicenseInfo = !string.IsNullOrEmpty(licenseUrl) ? ResourceHelper.GetString(licenseUrl!) : string.Empty;
        using Stream archiveStream = ResourceHelper.GetStream("pack://application:,,,/MicaSetup;component/Resources/Setups/publish.7z");

        ReaderOptions readerOptions = new()
        {
            LookForHeader = true,
            Password = string.IsNullOrEmpty(Option.Current.UnpackingPassword) ? null! : Option.Current.UnpackingPassword,
        };

        requestedFreeSpaceLong = ArchiveFileHelper.TotalUncompressSize(archiveStream, readerOptions) + (Option.Current.IsCreateUninst ? 2048000 : 0);
        RequestedFreeSpace = requestedFreeSpaceLong.ToFreeSpaceString();
        availableFreeSpaceLong = DriveInfoHelper.GetAvailableFreeSpace(installPath);
        AvailableFreeSpace = availableFreeSpaceLong.ToFreeSpaceString();
    }

    [ObservableProperty]
    private bool installPathShown = false;

    [RelayCommand]
    public void ShowOrHideInstallPath()
    {
        InstallPathShown = !InstallPathShown;
    }

    [RelayCommand]
    public void ShowOrHideLincenseInfo()
    {
        LicenseShown = !LicenseShown;
    }

    [RelayCommand]
    public void SelectFolder()
    {
        if (Option.Current.IsUseFolderPickerPreferClassic)
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
    public void StartInstall()
    {
        OnInstallPathChanged(InstallPath);

        if (IsIllegalPath)
        {
            _ = MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "IllegalPathTips".Tr());
            return;
        }

        try
        {
            if (requestedFreeSpaceLong >= availableFreeSpaceLong)
            {
                _ = MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "AvailableFreeSpaceInsufficientTips".Tr());
                return;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        try
        {
            if (!FileWritableHelper.CheckWritable(Path.Combine(InstallPath, Option.Current.ExeName)))
            {
                _ = MessageBox.Info(ApplicationDispatcherHelper.MainWindow, "LockedTipsAndExitTry".Tr(Option.Current.ExeName));
                return;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        Routing.GoToNext();
    }
}

partial class MainViewModel
{
    public string InstallPath
    {
        get => installPath;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(installPath, value))
            {
                OnInstallPathChanging(value);
                OnInstallPathChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("InstallPath"));
                installPath = value;
                OnInstallPathChanged(value);
                OnInstallPathChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("InstallPath"));
            }
        }
    }

    public string RequestedFreeSpace
    {
        get => requestedFreeSpace;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(requestedFreeSpace, value))
            {
                OnRequestedFreeSpaceChanging(value);
                OnRequestedFreeSpaceChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("RequestedFreeSpace"));
                requestedFreeSpace = value;
                OnRequestedFreeSpaceChanged(value);
                OnRequestedFreeSpaceChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("RequestedFreeSpace"));
            }
        }
    }

    public string AvailableFreeSpace
    {
        get => availableFreeSpace;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(availableFreeSpace, value))
            {
                OnAvailableFreeSpaceChanging(value);
                OnAvailableFreeSpaceChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("AvailableFreeSpace"));
                availableFreeSpace = value;
                OnAvailableFreeSpaceChanged(value);
                OnAvailableFreeSpaceChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("AvailableFreeSpace"));
            }
        }
    }

    public bool IsIllegalPath
    {
        get => isIllegalPath;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(isIllegalPath, value))
            {
                OnIsIllegalPathChanging(value);
                OnIsIllegalPathChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("IsIllegalPath"));
                isIllegalPath = value;
                OnIsIllegalPathChanged(value);
                OnIsIllegalPathChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("IsIllegalPath"));
            }
        }
    }

    public string LicenseInfo
    {
        get => licenseInfo;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(licenseInfo, value))
            {
                OnLicenseInfoChanging(value);
                OnLicenseInfoChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("LicenseInfo"));
                licenseInfo = value;
                OnLicenseInfoChanged(value);
                OnLicenseInfoChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("LicenseInfo"));
            }
        }
    }

    public bool LicenseShown
    {
        get => licenseShown;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(licenseShown, value))
            {
                OnLicenseShownChanging(value);
                OnLicenseShownChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("LicenseShown"));
                licenseShown = value;
                OnLicenseShownChanged(value);
                OnLicenseShownChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("LicenseShown"));
            }
        }
    }

    public bool LicenseRead
    {
        get => licenseRead;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(licenseRead, value))
            {
                OnLicenseReadChanging(value);
                OnLicenseReadChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("LicenseRead"));
                licenseRead = value;
                OnLicenseReadChanged(value);
                OnLicenseReadChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("LicenseRead"));
            }
        }
    }

    public bool CanStart
    {
        get => canStart;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(canStart, value))
            {
                OnCanStartChanging(value);
                OnCanStartChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("CanStart"));
                canStart = value;
                OnCanStartChanged(value);
                OnCanStartChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("CanStart"));
            }
        }
    }

    public bool IsElevated
    {
        get => isElevated;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(isElevated, value))
            {
                OnIsElevatedChanging(value);
                OnIsElevatedChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("IsElevated"));
                isElevated = value;
                OnIsElevatedChanged(value);
                OnIsElevatedChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("IsElevated"));
            }
        }
    }

    public bool IsCustomizeVisiableAutoRun
    {
        get => isCustomizeVisiableAutoRun;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(isCustomizeVisiableAutoRun, value))
            {
                OnIsCustomizeVisiableAutoRunChanging(value);
                OnIsCustomizeVisiableAutoRunChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("IsCustomizeVisiableAutoRun"));
                isCustomizeVisiableAutoRun = value;
                OnIsCustomizeVisiableAutoRunChanged(value);
                OnIsCustomizeVisiableAutoRunChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("IsCustomizeVisiableAutoRun"));
            }
        }
    }

    public bool AutoRun
    {
        get => autoRun;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(autoRun, value))
            {
                OnAutoRunChanging(value);
                OnAutoRunChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("AutoRun"));
                autoRun = value;
                OnAutoRunChanged(value);
                OnAutoRunChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("AutoRun"));
            }
        }
    }

    public bool DesktopShortcut
    {
        get => desktopShortcut;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(desktopShortcut, value))
            {
                OnDesktopShortcutChanging(value);
                OnDesktopShortcutChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("DesktopShortcut"));
                desktopShortcut = value;
                OnDesktopShortcutChanged(value);
                OnDesktopShortcutChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("DesktopShortcut"));
            }
        }
    }

    public bool InstallPathShown
    {
        get => installPathShown;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(installPathShown, value))
            {
                OnInstallPathShownChanging(value);
                OnInstallPathShownChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("InstallPathShown"));
                installPathShown = value;
                OnInstallPathShownChanged(value);
                OnInstallPathShownChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("InstallPathShown"));
            }
        }
    }

    partial void OnInstallPathChanging(string value);

    partial void OnInstallPathChanging(string? oldValue, string newValue);

    partial void OnInstallPathChanged(string value);

    partial void OnInstallPathChanged(string? oldValue, string newValue);

    partial void OnRequestedFreeSpaceChanging(string value);

    partial void OnRequestedFreeSpaceChanging(string? oldValue, string newValue);

    partial void OnRequestedFreeSpaceChanged(string value);

    partial void OnRequestedFreeSpaceChanged(string? oldValue, string newValue);

    partial void OnAvailableFreeSpaceChanging(string value);

    partial void OnAvailableFreeSpaceChanging(string? oldValue, string newValue);

    partial void OnAvailableFreeSpaceChanged(string value);

    partial void OnAvailableFreeSpaceChanged(string? oldValue, string newValue);

    partial void OnIsIllegalPathChanging(bool value);

    partial void OnIsIllegalPathChanging(bool oldValue, bool newValue);

    partial void OnIsIllegalPathChanged(bool value);

    partial void OnIsIllegalPathChanged(bool oldValue, bool newValue);

    partial void OnLicenseInfoChanging(string value);

    partial void OnLicenseInfoChanging(string? oldValue, string newValue);

    partial void OnLicenseInfoChanged(string value);

    partial void OnLicenseInfoChanged(string? oldValue, string newValue);

    partial void OnLicenseShownChanging(bool value);

    partial void OnLicenseShownChanging(bool oldValue, bool newValue);

    partial void OnLicenseShownChanged(bool value);

    partial void OnLicenseShownChanged(bool oldValue, bool newValue);

    partial void OnLicenseReadChanging(bool value);

    partial void OnLicenseReadChanging(bool oldValue, bool newValue);

    partial void OnLicenseReadChanged(bool value);

    partial void OnLicenseReadChanged(bool oldValue, bool newValue);

    partial void OnCanStartChanging(bool value);

    partial void OnCanStartChanging(bool oldValue, bool newValue);

    partial void OnCanStartChanged(bool value);

    partial void OnCanStartChanged(bool oldValue, bool newValue);

    partial void OnIsElevatedChanging(bool value);

    partial void OnIsElevatedChanging(bool oldValue, bool newValue);

    partial void OnIsElevatedChanged(bool value);

    partial void OnIsElevatedChanged(bool oldValue, bool newValue);

    partial void OnIsCustomizeVisiableAutoRunChanging(bool value);

    partial void OnIsCustomizeVisiableAutoRunChanging(bool oldValue, bool newValue);

    partial void OnIsCustomizeVisiableAutoRunChanged(bool value);

    partial void OnIsCustomizeVisiableAutoRunChanged(bool oldValue, bool newValue);

    partial void OnAutoRunChanging(bool value);

    partial void OnAutoRunChanging(bool oldValue, bool newValue);

    partial void OnAutoRunChanged(bool value);

    partial void OnAutoRunChanged(bool oldValue, bool newValue);

    partial void OnDesktopShortcutChanging(bool value);

    partial void OnDesktopShortcutChanging(bool oldValue, bool newValue);

    partial void OnDesktopShortcutChanged(bool value);

    partial void OnDesktopShortcutChanged(bool oldValue, bool newValue);

    partial void OnInstallPathShownChanging(bool value);

    partial void OnInstallPathShownChanging(bool oldValue, bool newValue);

    partial void OnInstallPathShownChanged(bool value);

    partial void OnInstallPathShownChanged(bool oldValue, bool newValue);
}

partial class MainViewModel
{
    private RelayCommand? showOrHideInstallPathCommand;
    public IRelayCommand ShowOrHideInstallPathCommand => showOrHideInstallPathCommand ??= new RelayCommand(ShowOrHideInstallPath);

    private RelayCommand? showOrHideLincenseInfoCommand;

    public IRelayCommand ShowOrHideLincenseInfoCommand => showOrHideLincenseInfoCommand ??= new RelayCommand(ShowOrHideLincenseInfo);

    private RelayCommand? selectFolderCommand;

    public IRelayCommand SelectFolderCommand => selectFolderCommand ??= new RelayCommand(SelectFolder);

    private RelayCommand? startInstallCommand;

    public IRelayCommand StartInstallCommand => startInstallCommand ??= new RelayCommand(StartInstall);
}
