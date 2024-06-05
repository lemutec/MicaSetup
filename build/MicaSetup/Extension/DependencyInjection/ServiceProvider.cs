using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MicaSetup.Extension.DependencyInjection;

public class ServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _singletonInstances = [];
    private readonly IServiceCollection _serviceCollection;

    [SuppressMessage("Style", "IDE0290:Use primary constructor")]
    public ServiceProvider(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public object GetService(Type serviceType)
    {
        var descriptor = _serviceCollection.FirstOrDefault(d => d.ServiceType == serviceType)
            ?? throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");

        if (descriptor.Lifetime == ServiceLifetime.Singleton)
        {
            if (!_singletonInstances.ContainsKey(serviceType))
            {
                _singletonInstances[serviceType] = CreateInstance(descriptor);
            }
            return _singletonInstances[serviceType];
        }

        return CreateInstance(descriptor);
    }

    private object CreateInstance(ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        var constructor = descriptor.ImplementationType!.GetConstructors().First();
        var parameters = constructor.GetParameters()
            .Select(p => GetService(p.ParameterType))
            .ToArray();
        return Activator.CreateInstance(descriptor.ImplementationType, parameters);
    }
}

public static class ServiceProviderExtensions
{
    public static object GetRequiredService(this IServiceProvider provider, Type type)
    {
        return provider.GetService(type);
    }

    public static TService GetRequiredService<TService>(this IServiceProvider provider)
    {
        return (TService)provider.GetService(typeof(TService));
    }
}
