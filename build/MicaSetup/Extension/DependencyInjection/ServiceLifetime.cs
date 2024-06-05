namespace MicaSetup.Extension.DependencyInjection;

/// <summary>
/// Specifies the lifetime of a service in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// Specifies that a single instance of the service will be created.
    /// </summary>
    Singleton,

    /// <summary>
    /// The same as <enum refs="Transient"/>.
    /// </summary>
    Scoped,

    /// <summary>
    /// Specifies that a new instance of the service will be created every time it is requested.
    /// </summary>
    Transient,
}
