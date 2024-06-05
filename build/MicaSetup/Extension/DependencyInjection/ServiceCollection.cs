using System;
using System.Collections.Generic;

namespace MicaSetup.Extension.DependencyInjection;

public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
{
}

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService implementationInstance) where TService : class
    {
        return Add(services, typeof(TService), implementationInstance);
    }

    public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
    {
        return Add(services, typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        return Add(services, serviceType, implementationType, ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
    {
        return Add(services, typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
    }

    public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        return Add(services, serviceType, implementationType, ServiceLifetime.Transient);
    }

    public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
    {
        return Add(services, typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        return Add(services, serviceType, implementationType, ServiceLifetime.Scoped);
    }

    public static IServiceCollection Add(this IServiceCollection collection, Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
        collection.Add(descriptor);
        return collection;
    }

    private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, object implementationInstance)
    {
        var descriptor = new ServiceDescriptor(serviceType, null, implementationInstance);
        collection.Add(descriptor);
        return collection;
    }

    public static IServiceProvider BuildServiceProvider(this IServiceCollection collection)
    {
        var serviceProvider = new ServiceProvider(collection);
        return serviceProvider;
    }
}
