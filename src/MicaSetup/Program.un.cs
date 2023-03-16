using MicaSetup.Controls;
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
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]

namespace MicaSetup;

internal class Program
{
    [STAThread]
    internal static void Main()
    {
        HostBuilderExtension
            .CreateBuilder()
            .UseAsUninst()
            .UseLogger()
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
                option.ExeName = "MicaApp.exe";
                option.KeyName = "MicaApp";
                option.DisplayName = "MicaApp";
                option.DisplayIcon = "MicaApp.exe";
                option.DisplayVersion = "1.2.0.0";
                option.Publisher = "Lemutec";
                option.AppName = "MicaApp";
                option.SetupName = $"MicaApp {Mui("UninstallProgram")}";
            })
            .UseServices(service =>
            {
                _ = service;
            })
            .CreateApp()
            .UseMuiLanguage()
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
