using Microsoft.Extensions.DependencyInjection;

namespace MicaSetup.Controls;

#pragma warning disable CS8618

public class HostBuilder : IHostBuilder
{
    public IApp App { get; set; }
    public ServiceProvider ServiceProvider { get; set; }

    public IHostBuilder CreateApp()
    {
        if (!string.IsNullOrEmpty(Option.Current.AppxPackageName) && Option.Current.AppxInstallMethod == Helper.MsixInstallMethod.AppInstaller)
        {
            App = new AppInstaller();
        }
        else
        {
            App = new App();
        }
        return this;
    }

    public void RunApp()
    {
        App?.Run();
    }
}
