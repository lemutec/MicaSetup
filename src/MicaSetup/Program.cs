using MicaSetup.Controls;
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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace MicaSetup;

internal class Program
{
    [STAThread]
    internal static void Main()
    {
        HostBuilderExtension
            .CreateBuilder()
            .UseLogger(true)
            .UseSingleInstance("MicaSetup")
            .UseTempPathFork()
            .UseElevated()
            .UsePack(pack =>
            {
                pack.DesktopShortcut = true;
                pack.CreateUninst = true;
                pack.RegistryKeys = true;
                pack.AutoRun = false;
                pack.FolderPickerPreferClassic = false;
                pack.InstallPathPreferX86 = false;
                pack.RegistryPreferX86 = null!;
                pack.AllowFullFolderSecurity = true;
                pack.ShowInstallingFileName = true;
                pack.OverlayInstallRemoveExt = "exe,dll,pdb";
                pack.ExeName = "MicaApp.exe";
                pack.KeyName = "MicaApp";
                pack.DisplayName = "MicaApp";
                pack.DisplayIcon = "MicaApp.exe";
                pack.DisplayVersion = "1.0.0.0";
                pack.Publisher = "Lemutec";
                pack.AppName = "MicaApp";
                pack.SetupName = $"MicaApp {Mui("Setup")}";
            })
            .CreateApp()
            .UseMuiLanguage()
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
