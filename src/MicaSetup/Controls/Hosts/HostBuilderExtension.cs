using MicaSetup.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MicaSetup.Controls;

public static class HostBuilderExtension
{
    public static IHostBuilder CreateBuilder()
    {
        HostBuilder builder = new();
        return builder;
    }

    public static IHostBuilder UseAsUninst(this IHostBuilder builder)
    {
        Pack.Current.Uninst = true;
        return builder;
    }

    public static IHostBuilder UseLogger(this IHostBuilder builder, bool enabled)
    {
        Pack.Current.Logging = enabled;
        Logger.Info("Setup run started ...");
        return builder;
    }

    public static IHostBuilder UseTempPathFork(this IHostBuilder builder)
    {
        if (RuntimeHelper.IsDebuggerAttached)
        {
            return builder;
        }
        TempPathForkHelper.Fork();
        return builder;
    }

    public static IHostBuilder UseElevated(this IHostBuilder builder)
    {
        RuntimeHelper.EnsureElevated();
        return builder;
    }

    public static IHostBuilder UseSingleInstance(this IHostBuilder builder, string instanceName, Action<bool> callback = null!)
    {
        RuntimeHelper.CheckSingleInstance(instanceName, callback);
        return builder;
    }

    public static IHostBuilder UseLangauge(this IHostBuilder builder, string name)
    {
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
        return builder;
    }

    public static IHostBuilder UseMuiLanguage(this IHostBuilder builder)
    {
        SetupLanguage();
        return builder;
    }

    public static IHostBuilder UsePack(this IHostBuilder builder, Action<Pack> handler)
    {
        handler?.Invoke(Pack.Current);
        return builder;
    }

    public static IHostBuilder UsePages(this IHostBuilder builder, Action<Dictionary<string, Type>> handler)
    {
        handler?.Invoke(ShellPageSetting.PageDict);
        return builder;
    }

    public static IHostBuilder UseDispatcherUnhandledExceptionCatched(this IHostBuilder builder, DispatcherUnhandledExceptionEventHandler handler = null!)
    {
        if (builder!.App != null)
        {
            if (handler != null)
            {
                builder!.App.DispatcherUnhandledException += handler;
            }
            else
            {
                builder!.App.DispatcherUnhandledException += (object s, DispatcherUnhandledExceptionEventArgs e) =>
                {
                    Logger.Fatal("Application.DispatcherUnhandledException", e?.Exception?.ToString()!);
                    e!.Handled = true;
                };
            }
        }
        return builder;
    }

    public static IHostBuilder UseDomainUnhandledExceptionCatched(this IHostBuilder builder, UnhandledExceptionEventHandler handler = null!)
    {
        if (handler != null)
        {
            AppDomain.CurrentDomain.UnhandledException += handler;
        }
        else
        {
            AppDomain.CurrentDomain.UnhandledException += (object s, UnhandledExceptionEventArgs e) =>
            {
                Logger.Fatal("AppDomain.CurrentDomain.UnhandledException", e?.ExceptionObject?.ToString()!);
            };
        }
        return builder;
    }

    public static IHostBuilder UseUnobservedTaskExceptionCatched(this IHostBuilder builder, EventHandler<UnobservedTaskExceptionEventArgs> handler = null!)
    {
        if (handler != null)
        {
            TaskScheduler.UnobservedTaskException += handler;
        }
        else
        {
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Logger.Fatal("TaskScheduler.UnobservedTaskException", e?.Exception?.ToString()!);
                e?.SetObserved();
            };
        }
        return builder;
    }
}
