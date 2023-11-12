using MicaSetup.Design.Controls;
using MicaSetup.Services;
using MicaSetup.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: AssemblyTitle("MicaApp Setup")]
[assembly: AssemblyProduct("MicaApp")]
[assembly: AssemblyDescription("MicaApp Setup")]
[assembly: AssemblyCompany("Lemutec")]
[assembly: AssemblyCopyright("Under MIT License. Copyright (c) Lemutec Contributors.")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

namespace MicaSetup;

internal class Program
{
    [STAThread]
    internal static void Main()
    {
        Hosting.CreateBuilder()
            .UseLogger()
            .UseSingleInstance("MicaSetup")
            .UseTempPathFork()
            .UseElevated()
            .UseDpiAware()
            .UseOptions(option =>
            {
                option.IsCreateDesktopShortcut = true;
                option.IsCreateUninst = true;
                option.IsCreateStartMenu = true;
                option.IsCreateQuickLaunch = false;
                option.IsCreateRegistryKeys = true;
                option.IsCreateAsAutoRun = true;
                option.IsCustomizeVisiableAutoRun = true;
                option.AutoRunLaunchCommand = "-autostart";
                option.UseFolderPickerPreferClassic = false;
                option.UseInstallPathPreferX86 = false;
                option.IsUseRegistryPreferX86 = null!;
                option.IsAllowFullFolderSecurity = true;
                option.IsAllowFirewall = true;
                option.IsRefreshExplorer = false;
                option.IsInstallCertificate = false;
                option.OverlayInstallRemoveExt = "exe,dll,pdb";
                option.UnpackingPassword = null!;
                option.ExeName = "MicaApp.exe";
                option.KeyName = "MicaApp";
                option.DisplayName = "MicaApp";
                option.DisplayIcon = "MicaApp.exe";
                option.DisplayVersion = "1.3.0.0";
                option.Publisher = "Lemutec";
                option.AppName = "MicaApp";
                option.SetupName = $"MicaApp {Mui("Setup")}";
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
