using System;

namespace MicaSetup.Services;

#pragma warning disable CS8618

public class ServiceManager
{
    public static IServiceProvider Services { get; set; }

    public static T? GetService<T>() where T : class
    {
        return Services.GetService(typeof(T)) as T;
    }
}
