using System.ComponentModel;

namespace MicaSetup.Core;

/// <summary>
/// Package Context
/// </summary>
public class Pack
{
    public static Pack Current { get; } = new();

    /// <summary>
    /// Indicates whether enable logger
    /// </summary>
    [Category("GlobalSetting")]
    public bool Logging { get; set; } = false;

    /// <summary>
    /// Indicates whether enable Mica Backdrop
    /// </summary>
    [Category("GlobalSetting")]
    public bool BackdropMica { get; set; } = false;

    /// <summary>
    /// Indicates whether App installing
    /// </summary>
    [Category("GlobalVariable")]
    public bool Installing { get; set; } = false;

    /// <summary>
    /// Indicates whether App uninstalling
    /// </summary>
    [Category("GlobalVariable")]
    public bool Uninstalling { get; set; } = false;

    /// <summary>
    /// Indicates whether this assembly as uninst
    /// </summary>
    [Category("GlobalSetting")]
    public bool Uninst { get; set; } = false;

    /// <summary>
    /// Indicates whether create uninst after app installed
    /// </summary>
    [Category("GlobalSetting")]
    public bool CreateUninst { get; set; } = true;

    /// <summary>
    /// Indicates whether to generate Desktop Shortcut
    /// </summary>
    [Category("GlobalSetting")]
    public bool DesktopShortcut { get; set; } = true;

    /// <summary>
    /// Indicates whether to generate Registry Keys
    /// </summary>
    [Category("GlobalSetting")]
    public bool RegistryKeys { get; set; } = true;

    /// <summary>
    /// Indicates whether to generate AutoRun Shortcut
    /// </summary>
    [Category("GlobalSetting")]
    public bool AutoRun { get; set; } = false;
    [Category("GlobalSetting")]
    public string AutoRunLaunchCommand { get; set; } = string.Empty;

    /// <summary>
    /// Prefer to provide classic type folder selector
    /// </summary>
    [Category("GlobalSetting")]
    public bool FolderPickerPreferClassic { get; set; } = false;

    /// <summary>
    /// Prefer to provide x86 type install path
    /// </summary>
    [Category("GlobalSetting")]
    public bool InstallPathPreferX86 { get; set; } = false;

    /// <summary>
    /// Prefer to provide x86 type Registry Key
    /// </summary>
    [Category("GlobalSetting")]
    public bool? RegistryPreferX86 { get; set; } = null!;

    /// <summary>
    /// Prefer to Allow Full Security intall path 
    /// </summary>
    [Category("GlobalSetting")]
    public bool AllowFullFolderSecurity { get; set; } = true;

    /// <summary>
    /// The file ext filter to remove when overlay install
    /// Using just like "exe,dll,pdb"
    /// </summary>
    [Category("GlobalSetting")]
    public string OverlayInstallRemoveExt { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether to show installing file name
    /// </summary>
    [Category("GlobalSetting")]
    public bool ShowInstallingFileName { get; set; } = true;

    /// <summary>
    /// Indicates whether to show uninstalling file name
    /// </summary>
    [Category("GlobalSetting")]
    public bool ShowUninstallingFileName { get; set; } = true;

    /// <summary>
    /// Your Product Exe file name
    /// </summary>
    [Category("GlobalSetting")]
    public string ExeName { get; set; } = null!;

    /// <summary>
    /// Indicates whether to uninst and keep my data
    /// </summary>
    [Category("GlobalVariable")]
    public bool KeepMyData { get; set; } = true;

    /// <summary>
    /// Registry Uninstall key name
    /// </summary>
    [Category("GlobalSetting")]
    public string KeyName { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `DisplayName` key value
    /// </summary>
    [Category("GlobalSetting")]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `DisplayIcon` key value
    /// </summary>
    [Category("GlobalSetting")]
    public string DisplayIcon { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `DisplayVersion` key value
    /// </summary>
    [Category("GlobalSetting")]
    public string DisplayVersion { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `InstallLocation` key value
    /// </summary>
    [Category("GlobalVariable")]
    public string InstallLocation { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `Publisher` key value
    /// </summary>
    [Category("GlobalSetting")]
    public string Publisher { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `UninstallString` key value
    /// </summary>
    [Category("GlobalVariable")]
    public string UninstallString { get; set; } = null!;

    /// <summary>
    /// Registry Uninstall `SystemComponent` key value
    /// true(1) => Hidden
    /// false(0) or _ => Shown (defalut)
    /// </summary>
    [Category("GlobalSetting")]
    public bool SystemComponent { get; set; } = false;

    /// <summary>
    /// Provide {AppName}.exe to auto run
    /// </summary>
    [Category("GlobalSetting")]
    public string AppName { get; set; } = null!;

    /// <summary>
    /// Provide SetupName
    /// </summary>
    [Category("GlobalSetting")]
    public string SetupName { get; set; } = null!;
}
