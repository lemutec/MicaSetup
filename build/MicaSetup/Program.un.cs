using MicaSetup.Design.Controls;
using MicaSetup.Extension.DependencyInjection;
using MicaSetup.Services;
using MicaSetup.Views;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("00000000-0000-0000-0000-000000000000")]
[assembly: AssemblyTitle("MicaApp Uninst")]
[assembly: AssemblyProduct("MicaApp")]
[assembly: AssemblyDescription("MicaApp Uninst")]
[assembly: AssemblyCompany("Lemutec")]
[assembly: AssemblyCopyright("Under MIT License. Copyright (c) Lemutec Contributors.")]
[assembly: AssemblyVersion("2.0.3.0")]
[assembly: AssemblyFileVersion("2.0.3.0")]

namespace MicaSetup;

internal class Program
{
    [STAThread]
    internal static void Main()
    {
        Hosting.CreateBuilder()
            .UseAsUninst()
            .UseLogger(false)
            .UseSingleInstance("MicaSetup")
            .UseTempPathFork()
            .UseElevated()
            .UseDpiAware()
            .UseOptions(option =>
            {
                option.IsCreateDesktopShortcut = true;
                option.IsCreateUninst = true;
                option.IsCreateRegistryKeys = true;
                option.IsCreateStartMenu = true;
                option.IsCreateQuickLaunch = false;
                option.IsCreateAsAutoRun = false;
                option.IsUseRegistryPreferX86 = null!;
                option.IsAllowFirewall = true;
                option.IsRefreshExplorer = true;
                option.IsInstallCertificate = false;
                option.ExeName = "MicaApp.exe";
                option.KeyName = "MicaApp";
                option.DisplayName = "MicaApp";
                option.DisplayIcon = "MicaApp.exe";
                option.DisplayVersion = "2.0.3.0";
                option.Publisher = "Lemutec";
                option.AppName = "MicaApp";
                option.SetupName = $"MicaApp {Mui("UninstallProgram")}";
                option.MessageOfPage1 = "MicaApp";
                option.MessageOfPage2 = Mui("ProgressTipsUninstalling");
                option.MessageOfPage3 = Mui("UninstallFinishTips");
            })
            .UseServices(service =>
            {
                service.AddSingleton<IMuiLanguageService, MuiLanguageService>();
                service.AddScoped<IExplorerService, ExplorerService>();
            })
            .CreateApp()
            .UseMuiLanguage()
            .UseTheme(WindowsTheme.Auto)
            .UsePages(page =>
            {
                page.Add(nameof(MainPage), typeof(MainPage));
                page.Add(nameof(UninstallPage), typeof(UninstallPage));
                page.Add(nameof(FinishPage), typeof(FinishPage));
            })
            .UseDispatcherUnhandledExceptionCatched()
            .UseDomainUnhandledExceptionCatched()
            .UseUnobservedTaskExceptionCatched()
            .RunApp();
    }
}
