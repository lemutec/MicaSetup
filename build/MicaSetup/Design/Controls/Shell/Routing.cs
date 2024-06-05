using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;

namespace MicaSetup.Design.Controls;

public static class Routing
{
    public static IRoutingProvider Provider { get; internal set; } = null!;
    public static WeakReference<ShellControl> Shell { get; internal set; } = null!;

    public static void RegisterRoute()
    {
        Provider ??= new RoutingProvider();

        foreach (var pageItem in ShellPageSetting.PageDict)
        {
            Provider.AddSingleton(pageItem.Key, pageItem.Value);
        }
    }

    public static FrameworkElement ResolveRoute(string route)
    {
        if (string.IsNullOrEmpty(route))
        {
            return null!;
        }
        return Provider?.Resolve<FrameworkElement>(route)!;
    }

    public static void GoTo(string route)
    {
        if (Shell != null)
        {
            if (Shell.TryGetTarget(out ShellControl shell))
            {
                OnGoTo(route);
                shell.Content = ResolveRoute(route);
                shell.Route = route;
            }
        }
    }

    public static void GoToNext()
    {
        if (Shell != null)
        {
            if (Shell.TryGetTarget(out ShellControl shell))
            {
                if (ShellPageSetting.PageDict.ContainsKey(shell.Route))
                {
                    bool found = false;
                    foreach (var item in ShellPageSetting.PageDict)
                    {
                        if (found)
                        {
                            OnGoTo(item.Key);
                            shell.Content = ResolveRoute(item.Key);
                            shell.Route = item.Key;
                            break;
                        }
                        if (item.Key == shell.Route)
                        {
                            found = true;
                        }
                    }
                }
            }
        }
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    private static void OnGoTo(string route)
    {
        ///
    }
}

public sealed class RoutingProvider : IRoutingProvider
{
    public List<RoutingServiceInfo> services = [];

    public T Resolve<T>(string name) where T : class
    {
        var serviceInfo = services.Where(x => x.Name == name).FirstOrDefault()
            ?? throw new InvalidOperationException($"Service '{name}' not found");

        return (T)Activator.CreateInstance(serviceInfo.Type);
    }

    public void AddSingleton(string name, Type type)
    {
        services.Add(new RoutingServiceInfo(name, type));
    }
}

public interface IRoutingProvider
{
    public T Resolve<T>(string name) where T : class;

    public void AddSingleton(string name, Type type);
}

public class RoutingServiceInfo(string name, Type type)
{
    public string Name { get; set; } = name;
    public Type Type { get; set; } = type;
}
