using System;

namespace MicaSetup.Design.Controls;

#pragma warning disable CS8618

public class HostBuilder : IHostBuilder
{
    public IApp App { get; set; }
    public IServiceProvider ServiceProvider { get; set; }

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
