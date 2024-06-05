using System;

namespace MicaSetup.Design.Controls;

public interface IHostBuilder
{
    public IApp App { get; set; }
    public IServiceProvider ServiceProvider { get; set; }

    public IHostBuilder CreateApp();

    public void RunApp();
}
