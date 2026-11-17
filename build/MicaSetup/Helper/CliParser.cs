using System;
using System.Collections.Generic;
using System.IO;

namespace MicaSetup.Helper;

/// <summary>
/// MSI-like setup CLI:
/// `/i` install, `/x` uninstall, `/d` install location,
/// `/q` or `/s` quiet, `/p` passive,
/// `/norestart`, `/forcerestart` and `NAME=value` public properties.
/// </summary>
public static class CliParser
{
    public const string Install = "i";
    public const string Uninstall = "x";
    public const string InstallDir = "d";
    public const string Quiet = "q";
    public const string Silent = "s";
    public const string Passive = "p";
    public const string NoRestart = "norestart";
    public const string ForceRestart = "forcerestart";
    public const string LoggingProperty = "LOGGING";

    public static CliUiMode UiMode
    {
        get
        {
            if (CommandLineHelper.Has(Quiet) || CommandLineHelper.Has(Silent))
            {
                return CliUiMode.Quiet;
            }
            if (CommandLineHelper.Has(Passive))
            {
                return CliUiMode.Passive;
            }
            return CliUiMode.Full;
        }
    }

    public static bool IsUnattended => UiMode != CliUiMode.Full;

    /// <summary>
    /// `LOGGING=1` (or true / yes / on) writes the log file.
    /// </summary>
    public static bool IsLoggingRequested
    {
        get
        {
            if (!CommandLineHelper.Properties.TryGetValue(LoggingProperty, out string? value))
            {
                return false;
            }

            return value.Trim().ToLowerInvariant() switch
            {
                "1" or "true" or "yes" or "on" => true,
                _ => false,
            };
        }
    }

    /// <summary>
    /// Apply CLI arguments to <paramref name="option"/>.
    /// </summary>
    /// <returns>False when the arguments are rejected.</returns>
    public static bool Apply(Option option)
    {
#if SETUP
        if (CommandLineHelper.Has(Uninstall))
        {
            Logger.Error($"[CLI] /{Uninstall} is not supported by setup, use the uninst program instead.");
            return false;
        }
#elif UNINST
        if (CommandLineHelper.Has(Install))
        {
            Logger.Error($"[CLI] /{Install} is not supported by uninst, use the setup program instead.");
            return false;
        }
#endif

        option.CliUiMode = UiMode;
        option.CliNoRestart = CommandLineHelper.Has(NoRestart);
        option.CliForceRestart = CommandLineHelper.Has(ForceRestart);

        if (option.CliNoRestart && option.CliForceRestart)
        {
            Logger.Warning($"[CLI] /{NoRestart} and /{ForceRestart} are both specified, /{NoRestart} takes precedence.");
            option.CliForceRestart = false;
        }

        foreach (KeyValuePair<string, string> property in CommandLineHelper.Properties)
        {
            option.CliProperties[property.Key] = property.Value;
        }

#if SETUP
        if (CommandLineHelper.Has(InstallDir))
        {
            string? installLocation = NormalizeInstallLocation(CommandLineHelper.Values[InstallDir]);

            if (installLocation == null)
            {
                Logger.Error($"[CLI] /{InstallDir} requires an absolute path.");
                return false;
            }
            option.InstallLocation = installLocation;
        }
#endif

        Logger.Information($"[CLI] Mode={option.CliUiMode}, NoRestart={option.CliNoRestart}, ForceRestart={option.CliForceRestart}, InstallLocation={option.InstallLocation}");
        return true;
    }

    /// <summary>
    /// Normalize the install location in the same way as the install path box on UI,
    /// so that `KeyName` is appended when the path does not end with it.
    /// </summary>
    /// <returns>Null when the path is not an absolute path.</returns>
    public static string? NormalizeInstallLocation(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            path = path.Trim().TrimEnd('\\', '/');
            if (path.EndsWith(":"))
            {
                path += Path.DirectorySeparatorChar;
            }

            if (!Path.IsPathRooted(path) || string.IsNullOrEmpty(Path.GetPathRoot(path)?.Trim('\\', '/')))
            {
                return null;
            }

            path = Path.GetFullPath(path);
            return path.EndsWith(Option.Current.KeyName, StringComparison.OrdinalIgnoreCase) ? path : Path.Combine(path, Option.Current.KeyName);
        }
        catch
        {
            return null;
        }
    }
}

public static class CliExitCode
{
    public const int Success = 0;
    public const int Failure = 1;
    public const int UserCancel = 1602;
    public const int AlreadyRunning = 1618;
    public const int RebootInitiated = 1641;
    public const int RebootRequired = 3010;
}
