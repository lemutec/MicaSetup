using System;
using System.IO;
using System.Reflection;

namespace MakeMica.Shared;

public static class MicaMacro
{
    public const string MicaDir = "${MicaDir}";
    public const string AppName = "${AppName}";
    public const string AppName1 = "${AppName:1}"; // Version::ToString(1)
    public const string AppName2 = "${AppName:2}"; // Version::ToString(2)
    public const string AppName3 = "${AppName:3}"; // Version::ToString(3)
    public const string AppName4 = "${AppName:4}"; // Version::ToString(4)
    public const string KeyName = "${KeyName}";
    public const string ExeName = "${ExeName}";
    public const string Version = "${Version}";
    public const string Package = "${Package}";

    public static string GetMicaDir()
        => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public static string GetFullPath(string? path)
    {
        string? newPath = path?.Replace(MicaDir, GetMicaDir());

        if (string.IsNullOrWhiteSpace(newPath))
        {
            return newPath!;
        }
        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Solve Template path.
    /// </summary>
    /// <param name="value">Recommend for ${MicaDir}/template/default.7z</param>
    public static string SolveTemplate(this string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        if (value.Contains(MicaDir))
        {
            string micadir = GetMicaDir();
            return value.Replace(MicaDir, micadir);
        }

        return value;
    }

    /// <summary>
    /// Solve Output setup path.
    /// </summary>
    /// <param name="value">Recommend for ./${AppName}_v${Version}_win64.exe</param>
    public static string SolveOutput(this string value, MicaConfig config)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        return
            value.Replace(AppName, config.AppName)
                 .Replace(KeyName, config.AppName)
                 .Replace(ExeName, config.AppName)
                 .Replace(Version, config.Version);
    }

    /// <summary>
    /// Solve Favicon/Icon/UnIcon path.
    /// </summary>
    /// <param name="value">Recommend for ${MicaDir}</param>
    public static string SolveIcon(this string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        if (value.Contains(MicaDir))
        {
            string micadir = GetMicaDir();
            return value.Replace(MicaDir, micadir);
        }

        return value;
    }

    /// <summary>
    /// Solve Version path.
    /// </summary>
    /// <param name="value">Recommend for ${Package}:${ExeName}</param>
    public static string SolveVersion(this string value, MicaConfig config)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        // TODO
        if (value.Contains(Package))
        {
        }

        return value;
    }
}
