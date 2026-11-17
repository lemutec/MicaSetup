using MicaSetup.Attributes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace MicaSetup.Helper;

public static class RuntimeHelper
{
    public static bool IsElevated { get; } = GetElevated();
    public static bool IsDebuggerAttached => Debugger.IsAttached;
    public static bool IsDesignMode { get; } = GetDesignMode();

    private static bool GetElevated()
    {
        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static bool GetDesignMode()
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return true;
        }
        else if (Process.GetCurrentProcess().ProcessName == "devenv")
        {
            return true;
        }
        return false;
    }

    public static void EnsureElevated()
    {
        if (IsDebuggerAttached)
        {
            Logger.Warning("[RuntimeHelper] IsDebuggerAttached causes skip EnsureElevated");
            return;
        }
        if (!IsElevated)
        {
            Restart();
        }
    }

    public static string ReArguments()
    {
        string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        for (int i = default; i < args.Length; i++)
        {
            args[i] = $@"""{args[i]}""";
        }
        return string.Join(" ", args);
    }

    public static void Restart(string fileName = null!, string dir = null!, string args = null!, int? exitCode = null, bool forced = false)
    {
        // Unattended CLI callers wait on this process, so it must stay alive and forward the child's exit code.
        bool waitForExit = CliParser.IsUnattended;
        FluentProcess process;

        try
        {
            if (waitForExit)
            {
                ReleaseSingleInstance();
            }

            process = FluentProcess.Create()
                .FileName(fileName ?? Path.Combine(dir ?? AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName))
                .Arguments(args ?? ReArguments())
                .WorkingDirectory(dir ?? Environment.CurrentDirectory)
                .UseShellExecute()
                .Verb(RequestExecutionLevelAttribute.IsRequestAdmin ? "runas" : string.Empty)
                .Start();
        }
        catch (Win32Exception e)
        {
            // The child was never started (such as UAC cancelled), so the forked copy is not locked and can be removed now.
            if (fileName != null)
            {
                try
                {
                    File.Delete(fileName);
                    string? forkedDir = Path.GetDirectoryName(fileName);
                    if (forkedDir != null && Directory.Exists(forkedDir) && Directory.GetFileSystemEntries(forkedDir).Length == 0)
                    {
                        Directory.Delete(forkedDir);
                    }
                }
                catch
                {
                }
            }

            if (waitForExit)
            {
                // The single instance handle is released already, so never continue in this process.
                Logger.Error(e);
                TempPathForkHelper.Clean();
                Environment.Exit(e.NativeErrorCode == ErrorCancelled ? CliExitCode.UserCancel : CliExitCode.Failure);
            }
            return;
        }
        if (waitForExit)
        {
            process.WaitForExit();
            Environment.Exit(process.ExitCode);
        }
        if (forced)
        {
            Process.GetCurrentProcess().Kill();
        }
        Environment.Exit(exitCode ?? 'r' + 'u' + 'n' + 'a' + 's');
    }

    private const int ErrorCancelled = 1223;

    private static EventWaitHandle? singleInstanceHandle;
    private static Task? singleInstanceTask;
    private static volatile bool singleInstanceReleased;

    public static void CheckSingleInstance(string instanceName, Action<bool> callback = null!)
    {
        EventWaitHandle? handle;

        try
        {
            handle = EventWaitHandle.OpenExisting(instanceName);
            handle.Set();
            callback?.Invoke(false);
            Environment.Exit(CliParser.IsUnattended ? CliExitCode.AlreadyRunning : 0xFFFF);
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            callback?.Invoke(true);
            handle = new EventWaitHandle(false, EventResetMode.AutoReset, instanceName);
        }
        singleInstanceHandle = handle;
        singleInstanceTask = Task.Run(() =>
        {
            while (handle.WaitOne() && !singleInstanceReleased)
            {
                // Skip activation when there is no UI, such as quiet mode.
                if (Option.Current.CliUiMode == CliUiMode.Quiet)
                {
                    continue;
                }
                ApplicationDispatcherHelper.BeginInvoke(main =>
                {
                    main?.Activate();
                    main?.Show();
                });
            }
        });
    }

    /// <summary>
    /// Close the single instance handle so that a restarted child process can own it.
    /// </summary>
    public static void ReleaseSingleInstance()
    {
        if (singleInstanceHandle == null)
        {
            return;
        }

        singleInstanceReleased = true;
        _ = singleInstanceHandle.Set();
        _ = singleInstanceTask?.Wait(1000);
        singleInstanceHandle.Dispose();
        singleInstanceHandle = null;
    }
}
