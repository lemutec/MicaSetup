using MicaSetup.Helper;
using System;
using System.Windows;

namespace MicaSetup;

public partial class App : Application, IApp
{
    public App()
    {
        InitializeComponent();

        if (Option.Current.IsCliUnattended)
        {
            Startup += OnUnattendedStartup;
        }
    }

    private async void OnUnattendedStartup(object sender, StartupEventArgs e)
    {
        if (Option.Current.CliUiMode == CliUiMode.Quiet)
        {
            // No window will ever be shown, so any missed shutdown leaves an invisible process behind.
            StartupUri = null;
            int exitCode = CliExitCode.Failure;
            try
            {
                exitCode = await SetupPipelineHelper.RunUnattendedAsync();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            SetupPipelineHelper.Exit(exitCode);
            return;
        }

        bool prepared;
        try
        {
#if SETUP
            prepared = SetupPipelineHelper.PrepareInstall();
#elif UNINST
            prepared = SetupPipelineHelper.PrepareUninstall();
#else
            prepared = true;
#endif
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex);
            prepared = false;
        }

        if (!prepared)
        {
            StartupUri = null;
            SetupPipelineHelper.Exit(CliExitCode.Failure);
        }
    }
}

public interface IApp
{
    public int Run();
}
