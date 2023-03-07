using System;
using System.Windows;
using Autofac;

namespace MicaSetup.Controls;

public static class Routing
{
    public static ContainerBuilder Builder { get; internal set; } = null!;
    public static IContainer Container { get; internal set; } = null!;
    public static WeakReference<ShellControl> Shell { get; internal set; } = null!;

    public static ContainerBuilder CreateBuilder()
    {
        return Builder ??= new();
    }

    public static ContainerBuilder Register(string route, Type type)
    {
        return Builder.Register(route, type);
    }

    public static void Build()
    {
        Container = Builder.Build();
    }

    public static FrameworkElement ResolveRoute(string route)
    {
        if (string.IsNullOrEmpty(route))
        {
            return null!;
        }
        return Container.ResolveOptionalNamed<FrameworkElement>(route)!;
    }

    public static void GoTo(string route)
    {
        if (Shell != null)
        {
            if (Shell.TryGetTarget(out ShellControl shell))
            {
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
}

file static class RoutingExtension
{
    public static ContainerBuilder Register(this ContainerBuilder builder, string route, Type type)
    {
        builder.RegisterType(type).Named<FrameworkElement>(route);
        return builder;
    }
}
