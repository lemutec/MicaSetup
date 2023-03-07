namespace MicaSetup.Controls;

public class HostBuilder : IHostBuilder
{
    public App? App { get; set; }

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
