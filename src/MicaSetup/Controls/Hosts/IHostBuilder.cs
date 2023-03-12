using Microsoft.Extensions.DependencyInjection;

namespace MicaSetup.Controls;

public interface IHostBuilder
{
    public App App { get; set; }
    public ServiceProvider ServiceProvider { get; set; }

    public IHostBuilder CreateApp();
    public void RunApp();
}
