using Microsoft.Extensions.DependencyInjection;

namespace MicaSetup.Controls;

#pragma warning disable CS8618

public class HostBuilder : IHostBuilder
{
    public App App { get; set; }
    public ServiceProvider ServiceProvider { get; set; }

    public IHostBuilder CreateApp()
    {
        App = new App();
        return this;
    }

    public void RunApp()
    {
        App?.Run();
    }
}
