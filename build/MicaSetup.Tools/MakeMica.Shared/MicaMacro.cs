using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MakeMica.Shared;

public static class MicaMacro
{
    public const string MicaDir = "${MicaDir}";
    public const string AppName = "${AppName}";
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
                 .Replace(KeyName, config.KeyName)
                 .Replace(ExeName, config.ExeName)
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
    /// <param name="config"></param>
    /// <param name="solvePackage">function solvePackage(option: {filePath: string, targetEntryKey: string}) => value</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string SolveVersion(this string value, MicaConfig config, Func<string, string, string>? solvePackageFunc = null)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        if (value.Contains(Package))
        {
            value = value
                .Replace(Package, string.Empty) // Remove marco mark.
                .Replace(AppName, config.AppName)
                .Replace(KeyName, config.KeyName)
                .Replace(ExeName, config.ExeName);

            int? ifFieldCount = null;

            Match match = Regex.Match(value, @"\|(\d+)$");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int fieldCount))
                {
                    ifFieldCount = fieldCount;
                    value = Regex.Replace(value, @"\|(\d+)$", string.Empty);
                }
            }

            Console.WriteLine("Solve ${Version} from ${Package}.");
            Console.WriteLine("If your package is too big, it may require some memory and time.");
            Console.WriteLine("Use file system instead if not worked or crashed.");

            // Should fallback to `v1.0.0`.
            value = solvePackageFunc?.Invoke(config.Package, value) ?? "1.0.0";

            if (ifFieldCount != null)
            {
                value = new Version(value).ToString(ifFieldCount.Value);
            }
        }
        else if (value.Contains(AppName) || value.Contains(KeyName) || value.Contains(ExeName))
        {
            value = value
                .Replace(AppName, config.AppName)
                .Replace(KeyName, config.KeyName)
                .Replace(ExeName, config.ExeName);

            int? ifFieldCount = null;

            Match match = Regex.Match(value, @"\|(\d+)$");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int fieldCount))
                {
                    ifFieldCount = fieldCount;
                    value = Regex.Replace(value, @"\|(\d+)$", string.Empty);
                }
            }

            if (!File.Exists(value))
            {
                throw new FileNotFoundException(value);
            }

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(value);
            value = fileVersionInfo.FileVersion;

            if (ifFieldCount != null)
            {
                value = new Version(value).ToString(ifFieldCount.Value);
            }
        }

        return value;
    }
}
