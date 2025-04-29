namespace MakeMica.Shared;

public class MicaConfig
{
    /// <summary>
    /// The template file location.
    /// When `.dist` dir is empty, extract it to `.dist`.
    /// Support 7z/zip template.
    /// </summary>
    public string Template { get; set; } = "${MicaDir}/template/default.7z";

    /// <summary>
    /// The package file location.
    /// Support 7z/zip file.
    /// </summary>
    public string Package { get; set; } = "./publish.7z";

    /// <summary>
    /// The output setup file location.
    /// </summary>
    public string Output { get; set; } = "./${AppName}Setup_v${Version}.exe";

    /// <summary>
    /// The display name of register.
    /// </summary>
    public string AppName { get; set; } = "MicaApp";

    /// <summary>
    /// The key name of register.
    /// </summary>
    public string KeyName { get; set; } = "MicaApp";

    /// <summary>
    /// Your application location in package.
    /// </summary>
    public string ExeName { get; set; } = "MicaApp.exe";

    /// <summary>
    /// The publisher name of register.
    /// </summary>
    public string Publisher { get; set; } = "Lemutec";

    /// <summary>
    /// Target setup and display version of register.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// .NET Framework version.
    /// Only support `net472`, `net48` and `net481`.
    /// </summary>
    public string TargetFramework { get; set; } = "net472";

    /// <summary>
    /// Assembly GUID for the setup.
    /// </summary>
    public string Guid { get; set; } = "00000000-0000-0000-0000-000000000000";

    /// <summary>
    /// The main icon location.
    /// Only support extension file of png/ico.
    /// When null, use internal main icon.
    /// </summary>
    public string? Favicon { get; set; } = null;

    /// <summary>
    /// The setup icon location.
    /// Only support extension file of png/ico.
    /// When null, use internal setup icon.
    /// </summary>
    public string? Icon { get; set; } = null;

    /// <summary>
    /// The uninst icon location.
    /// Only support extension file of png/ico.
    /// When null, use internal uninst icon.
    /// </summary>
    public string? UnIcon { get; set; } = null;

    /// <summary>
    /// The setup LICENSE file location.
    /// UTF-8 is needed.
    /// When null, use internal license file.
    /// </summary>
    public string? LicenseFile { get; set; } = null;

    /// <summary>
    /// The setup LICENSE text.
    /// When null, use license file.
    /// </summary>
    public string? License { get; set; } = null;

    /// <summary>
    /// TODO
    /// </summary>
    public string? LicenseType { get; set; } = null;

    /// <summary>
    /// Indicate the permissions of setup and uninst.
    /// Only support `admin` and `user`.
    /// When user, need to know the following.
    ///     1. The setup and uninst will not require admin permissions.
    ///     2. The options that require admin permissions will be invalid.
    ///     3. The setup will create a file named `uninst.dat` for uninst.
    /// </summary>
    public string RequestExecutionLevel { get; set; } = "admin";

    /// <summary>
    /// The mutex name used for single instance.
    /// When null, use the auto name based on key name.
    /// </summary>
    public string? SingleInstanceMutex { get; set; } = null;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCreateDesktopShortcut"/>
    /// </summary>
    public bool IsCreateDesktopShortcut { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCreateUninst"/>
    /// </summary>
    public bool IsCreateUninst { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsUninstLower"/>
    /// </summary>
    public bool IsUninstLower { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCreateStartMenu"/>
    /// </summary>
    public bool IsCreateStartMenu { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsPinToStartMenu"/>
    /// </summary>
    public bool IsPinToStartMenu { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCreateQuickLaunch"/>
    /// </summary>
    public bool IsCreateQuickLaunch { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCreateRegistryKeys"/>
    /// </summary>
    public bool IsCreateRegistryKeys { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCreateAsAutoRun"/>
    /// </summary>
    public bool IsCreateAsAutoRun { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsCustomizeVisiableAutoRun"/>
    /// </summary>
    public bool IsCustomizeVisiableAutoRun { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.AutoRunLaunchCommand"/>
    /// </summary>
    public string AutoRunLaunchCommand { get; set; } = "-autostart";

    /// <summary>
    /// <see cref="MicaSetup.Option.IsUseFolderPickerPreferClassic"/>
    /// </summary>
    public bool IsUseFolderPickerPreferClassic { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsUseInstallPathPreferX86"/>
    /// </summary>
    public bool IsUseInstallPathPreferX86 { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsUseInstallPathPreferAppDataLocalPrograms"/>
    /// </summary>
    public bool IsUseInstallPathPreferAppDataLocalPrograms { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsUseInstallPathPreferAppDataRoaming"/>
    /// </summary>
    public bool IsUseInstallPathPreferAppDataRoaming { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsUseRegistryPreferX86"/>
    /// </summary>
    public bool? IsUseRegistryPreferX86 { get; set; } = null!;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsAllowFullFolderSecurity"/>
    /// </summary>
    public bool IsAllowFullFolderSecurity { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsAllowFirewall"/>
    /// </summary>
    public bool IsAllowFirewall { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsRefreshExplorer"/>
    /// </summary>
    public bool IsRefreshExplorer { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsInstallCertificate"/>
    /// </summary>
    public bool IsInstallCertificate { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.IsEnableUninstallDelayUntilReboot"/>
    /// </summary>
    public bool IsEnableUninstallDelayUntilReboot { get; set; } = true;

    /// <summary>
    /// <see cref="MicaSetup.Option.OverlayInstallRemoveExt"/>
    /// </summary>
    public string OverlayInstallRemoveExt { get; set; } = "exe,dll,pdb";

    /// <summary>
    /// <see cref="MicaSetup.Option.UnpackingPassword"/>
    /// </summary>
    public string? UnpackingPassword { get; set; } = null;

    /// <summary>
    /// Indicates whether to add environment variable
    /// </summary>
    public bool IsEnvironmentVariable { get; set; } = false;

    /// <summary>
    /// <see cref="MicaSetup.Option.MessageOfPage1"/>
    /// </summary>
    public string? MessageOfPage1 { get; set; } = null;

    /// <summary>
    /// <see cref="MicaSetup.Option.MessageOfPage2"/>
    /// </summary>
    public string? MessageOfPage2 { get; set; } = null;

    /// <summary>
    /// <see cref="MicaSetup.Option.MessageOfPage3"/>
    /// </summary>
    public string? MessageOfPage3 { get; set; } = null;
}
