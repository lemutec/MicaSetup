![logo64](https://raw.githubusercontent.com/lemutec/MicaSetup/refs/heads/v2/branding/logo64.png)

[![GitHub license](https://img.shields.io/github/license/lemutec/MicaSetup)](https://github.com/lemutec/MicaSetup/blob/v2/LICENSE) [![NuGet](https://img.shields.io/nuget/v/MicaSetup.Tools.svg)](https://nuget.org/packages/MicaSetup.Tools) [![Actions](https://github.com/lemutec/MicaSetup/actions/workflows/micasetup.yml/badge.svg)](https://github.com/lemutec/MicaSetup/actions/workflows/micasetup.yml) [![Platform](https://img.shields.io/badge/platform-Windows-blue?logo=windowsxp&color=1E9BFA)](https://dotnet.microsoft.com/zh-cn/download/dotnet/latest/runtime)

# 📦MicaSetup

MicaSetup is a universal advanced installation package generation tool project that supports various features and allows customization of the full entire installation process.

## 🔣Toolkits

| Toolkits | Description                                               | GUI               | CLI               |
| -------- | --------------------------------------------------------- | ----------------- | ----------------- |
| MakeIcon | Create setup and uninst icon, both *.png and *.ico.       | Ready             | Ready             |
| MakeMica | Build a package installer using configuration JSON file.  | Under development | Under development |
| MakeMui  | A GUI for editing the multi-language XAML resource files. | Under development | /                 |

## 🌒ScreenShots

<img src="https://raw.githubusercontent.com/lemutec/MicaSetup/refs/heads/v2/assets/image-20240520214715174.png" alt="image-20240520214715174" style="zoom: 80%;" />

<img src="https://raw.githubusercontent.com/lemutec/MicaSetup/refs/heads/v2/assets/image-20240520214758733.png" alt="image-20240520214758733" style="zoom:80%;" />

## 💡Features

### 🌟Advanced Features

| Features            | Completion | Addition                                                     |
| ------------------- | ---------- | ------------------------------------------------------------ |
| Mica Backdrop       | ☑          | Support Windows11 Mica Backdrop.                             |
| Theme               | ☑          | Support Dark and Light theme.                                |
| Archive File        | ☑          | Pack your app with `7z.exe`, so it's easy to customize your archive file or not. |
| Animations          | ☑          | Support a lot of advanced animations.<br />Such as Indeterminate ProgressBar, supported when your installation is no progress hint. |
| ~~Notification~~    | ~~☑~~      | ~~Windows10 (or higher) Notification.~~                      |
| HiDPI               | ☑          | Support to process DPI awareness.                            |
| Firewall            | ☑          | Support allows your app to access the Network through Windows Firewall Policy. |
| Anti Unpacking      | ☑          | Support encryption type archive file package.<br />Needs create your app archive file with password firstly.<br />Also can sign your setup program by `.snk`. |
| .NET SDKs Installer | 🔲          | Auto detect and install the .NET/.NET Core/.NET Framework Runtime.<br />[Download .NET SDKs for Visual Studio](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks) |
| MSIX Installer      | 🔲          | Using MSIX package instead of archive file.<br />Provide the function the same as following command.<br />`certmgr.exe -add app.cer -s -r localMachine AuthRoot`. |
| Online Installer    | 🔲          | Using Network instead of archive file.                       |

### ✨General Features

| Features           | Completion | Addition                                                     |
| ------------------ | ---------- | ------------------------------------------------------------ |
| Host Builder       | ☑          | Support a simple HostBuilder for handling setup configs.     |
| Debugger           | ☑          | Unlike other installation architectures, too difficult to debug.<br />Support using VS2022 (or higher) to debug codes. |
| WindowChrome       | ☑          | Support WindowChrome to keep native window style instead of borderless style. |
| Shortcut           | ☑          | Support to create your app shortcut (`.lnk`).                |
| Fork Process       | ☑          | Support setup process forked itself to avoid the file lock.  |
| Folder Picker      | ☑          | Support advanced Folder Picker by `WindowsAPICodePack-Shell`. |
| Globalization      | ☑          | Support English, Chinese and Japanese nowaday.<br />Support other languages customized by yourself. |
| Registry           | ☑          | Support x86/x64 registry.                                    |
| Create StartMenu   | ☑          | Support to create Start Menu in system folder.               |
| Create QuickLaunch | ☑          | Support to create Quick Launch shortcut (`.lnk`).<br />Quick Launch has been obsoleted since Windows 10. |
| Create Uninst      | ☑          | Support to create `Uninst.exe` for system uninstallation.    |
| Delay Uninst       | ☑          | Support to delete file delay until reboot.<br />Useful for handling when uninst files fails. |
| Auto Run           | ☑          | Support to make your installed app auto run.                 |
| Security Control   | ☑          | Support to make your installed path Allow Full File Security. |
| Install Font       | 🔲          | Support to install your fonts into system.                   |
| Install Silent     | 🔲          | Support silent installation using CLI `/q`.<br />Useful for auto updating in the background. |
| Install Automate   | 🔲          | Support no interactive installation using CLI `/a`.<br />Useful for auto updating in the foreground. |

## 🎛️Personalized Controls

| Controls           | Completion | Addition                                          |
| ------------------ | ---------- | ------------------------------------------------- |
| WindowX            | ☑          | Support Mica Backdrop.<br />Support WindowChrome. |
| SetupProgressBar   | ☑          | Support Indeterminate Animation.                  |
| MessageBoxX        | ☑          | Simple MessageBox instead of Classics.            |
| SmoothScrollViewer | ☑          | Support smoothly scrolling.                       |

## 😋Demos

> The programs using MicaSetup.

[QL-Win/QuickLook](https://github.com/QL-Win/QuickLook) (Recommended)

[GenshinMatrix/Fischless.Relauncher](https://github.com/GenshinMatrix/Fischless.Relauncher)

[lemutec/LyricStudio](https://github.com/lemutec/LyricStudio)

[GenshinMatrix/Fischless](https://github.com/GenshinMatrix/Fischless)

## 📖Usage & Wikis

### Build via NuGet package

```xml
<ItemGroup>
    <PackageReference Include="MicaSetup.Tools" Version="2.5.0" />
</ItemGroup>
```

and then you can use `makemica.exe` and `7z.exe` from NuGet cache directory in your PowerShell script.

```powershell
Write-Host "Get NuGet global package cache directory"
$globalPackages = (dotnet nuget locals global-packages --list) -split ":\s*", 2 | Select-Object -Last 1
$sevenZip = Join-Path $globalPackages "micasetup.tools\2.5.0\build\bin\7z.exe"
$exePath = Join-Path $globalPackages "micasetup.tools\2.5.0\build\makemica.exe"

Write-Host "7z packing"
& $sevenZip a $package7z ".\*" -t7z -mx=5 -mf=BCJ2 -r -y

Write-Host "Installer building"
# Ensure your micasetup.json file here
& $exePath
```

### Usage of `fetchver`

```bash
@echo off
for /f "tokens=* delims=" %%i in ('fetchver.exe /v3 "fetchver.exe"') do set fetchver=%%i
@echo The result is: %fetchver%
@pause
```

Support options: v1, v2, v3, v4

## 🎃Developer

### ⚙️Debug Codes

Run the `setup_dummy.cmd` for creating some dummy things used in project.

Such as `publish.7z` as your app published package and full size font used in Setup programs.

And then Run VS2026 (or higher) as Admin to open the `.slnx`.

### 📦Create Setup

Run the `setup_build.cmd` for building the full Setup programs.

Command Flow:

1. Build my app, named `MicaApp` using `dotnet publish`.
2. Pack my app using `7z.exe`.
3. Build Uninst, named `Uninst.exe` using `msbuild`.
4. Build Setup, named `MicaSetup.exe` using `msbuild`.

## 🚀Contributes

🈺Welcome PRs, issues, wikis, demos.

## 🌟Components

Including follow [components](COMPONENTS.md).

## 📑License

This software codes are licensed under the [MIT](LICENSE) License.

