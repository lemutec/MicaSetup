﻿using MicaSetup.Attributes;
using MicaSetup.Design.Controls;
using MicaSetup.Extension.DependencyInjection;
using MicaSetup.Services;
using MicaSetup.Views;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: AssemblyTitle("MicaApp Setup")]
[assembly: AssemblyProduct("MicaApp")]
[assembly: AssemblyDescription("MicaApp Setup")]
[assembly: AssemblyCompany("Lemutec")]
[assembly: AssemblyCopyright("Under MIT License. Copyright (c) Lemutec Contributors.")]
[assembly: AssemblyVersion("2.1.1.0")]
[assembly: AssemblyFileVersion("2.1.1.0")]
[assembly: RequestExecutionLevel("admin")]

namespace MicaSetup;

internal class Program
{
    [STAThread]
    internal static void Main()
    {
        Hosting.CreateBuilder()
            .UseLogger(false)
            .UseSingleInstance("MicaSetup")
            .UseTempPathFork()
            .UseElevated()
            .UseDpiAware()
            .UseOptions(option =>
            {
                option.IsCreateDesktopShortcut = true;
                option.IsCreateUninst = true;
                option.IsCreateStartMenu = true;
                option.IsPinToStartMenu = false;
                option.IsCreateQuickLaunch = true;
                option.IsCreateRegistryKeys = true;
                option.IsCreateAsAutoRun = true;
                option.IsCustomizeVisiableAutoRun = true;
                option.AutoRunLaunchCommand = "-autostart";
                option.IsUseFolderPickerPreferClassic = false;
                option.IsUseInstallPathPreferX86 = false;
                option.IsUseRegistryPreferX86 = null!;
                option.IsAllowFullFolderSecurity = true;
                option.IsAllowFirewall = true;
                option.IsRefreshExplorer = true;
                option.IsInstallCertificate = false;
                option.IsEnableUninstallDelayUntilReboot = true;
                option.IsUseLicenseFile = false;
                option.OverlayInstallRemoveExt = "exe,dll,pdb";
                option.OverlayInstallRemoveHandler = null!;
                option.UnpackingPassword = null!;
                option.AppName = "MicaApp";
                option.KeyName = "MicaApp";
                option.ExeName = "MicaApp.exe";
                option.DisplayName = $"{option.AppName}";
                option.DisplayIcon = $"{option.ExeName}";
                option.DisplayVersion = "2.1.1.0";
                option.Publisher = "Lemutec";
                option.SetupName = $"{option.AppName} {Mui("Setup")}";
                option.MessageOfPage1 = $"{option.AppName}";
                option.MessageOfPage2 = Mui("Installing");
                option.MessageOfPage3 = Mui("InstallFinishTips");
            })
            .UseServices(service =>
            {
                service.AddSingleton<IMuiLanguageService, MuiLanguageService>();
                service.AddScoped<IDotNetVersionService, DotNetVersionService>();
                service.AddScoped<IExplorerService, ExplorerService>();
            })
            .CreateApp()
            .UseMuiLanguage()
            .UseTheme(WindowsTheme.Auto)
            .UsePages(page =>
            {
                page.Add(nameof(MainPage), typeof(MainPage));
                page.Add(nameof(InstallPage), typeof(InstallPage));
                page.Add(nameof(FinishPage), typeof(FinishPage));
            })
            .UseDispatcherUnhandledExceptionCatched()
            .UseDomainUnhandledExceptionCatched()
            .UseUnobservedTaskExceptionCatched()
            .RunApp();
    }
}
