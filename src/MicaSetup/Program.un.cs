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
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]

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
            .UsePack(pack =>
            {
                pack.IsCreateDesktopShortcut = true;
                pack.IsCreateUninst = true;
                pack.IsCreateRegistryKeys = true;
                pack.IsCrateAsAutoRun = false;
                pack.IsUseRegistryPreferX86 = null!;
                pack.ExeName = "MicaApp.exe";
                pack.KeyName = "MicaApp";
                pack.DisplayName = "MicaApp";
                pack.DisplayIcon = "MicaApp.exe";
                pack.DisplayVersion = "1.0.0.0";
                pack.Publisher = "Lemutec";
                pack.AppName = "MicaApp";
                pack.SetupName = $"MicaApp {Mui("UninstallProgram")}";
            })
            .UseServices(service =>
            {
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
