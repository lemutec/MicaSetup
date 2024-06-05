using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MicaSetup.Extension.DependencyInjection;

[DebuggerDisplay("{ToString()}")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
public class ServiceDescriptor
{
    private Type? _implementationType = null;
    public Type? ImplementationType => _implementationType;

    private object? _implementationInstance = null;
    public object? ImplementationInstance => _implementationInstance;

    public ServiceLifetime Lifetime { get; }
    public object? ServiceKey { get; }
    public Type ServiceType { get; }

    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        : this(serviceType, null, implementationType, lifetime)
    {
    }

    public ServiceDescriptor(Type serviceType, object? serviceKey, Type implementationType, ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
        ServiceType = serviceType;
        ServiceKey = serviceKey;
        _implementationType = implementationType;
    }

    public ServiceDescriptor(Type serviceType, object? serviceKey, object instance)
    {
        Lifetime = ServiceLifetime.Singleton;
        ServiceType = serviceType;
        ServiceKey = serviceKey;
        _implementationInstance = instance;
    }

    public override string ToString()
    {
        string text = $"{"ServiceType"}: {ServiceType} {"Lifetime"}: {Lifetime} ";

        if (ImplementationType != null)
        {
            return text + $"{"ImplementationType"}: {ImplementationType}";
        }

        return text + $"{"ImplementationInstance"}: {ImplementationInstance}";
    }
}
