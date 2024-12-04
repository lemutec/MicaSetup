using FetchVer.Helper;
using System.Diagnostics;

namespace FetchVer;

internal sealed class CommandLine
{
    public int? FieldCount { get; set; } = null;

    public string? OriginalPath { get; set; }

    public string? PackagePath { get; set; }

    public string? SourcePath { get; set; }

    public OutputType OutputType { get; set; } = OutputType.StdOut; // TODO

    public string? OutputPath { get; set; } // TODO

    public void Execute()
    {
        if (!string.IsNullOrWhiteSpace(PackagePath))
        {
            if (PackagePath != null && SourcePath != null && File.Exists(PackagePath))
            {
                string? versionString = PEImageHelper.GetFileVersionFromArchive(PackagePath, SourcePath);

                if (versionString != null)
                {
                    if (FieldCount != null)
                    {
                        Console.WriteLine(new Version(versionString).ToString(FieldCount.Value));
                        return;
                    }
                    else
                    {
                        Console.WriteLine(new Version(versionString).ToString());
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: Version not found in {SourcePath}");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"Error: Package file not found: {PackagePath}");
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(SourcePath))
        {
            if (SourcePath != null && File.Exists(SourcePath))
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(SourcePath);
                string? versionString = fileVersionInfo.FileVersion;

                if (versionString != null)
                {
                    if (FieldCount != null)
                    {
                        Console.WriteLine(new Version(versionString).ToString(FieldCount.Value));
                        return;
                    }
                    else
                    {
                        Console.WriteLine(new Version(versionString).ToString());
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: Version not found in {SourcePath}");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"Error: Source file not found: {SourcePath}");
                return;
            }
        }
    }
}

internal enum OutputType
{
    StdOut = 0,
    StdError = 1,
    TextFile = 2,
}

internal static class ParseCommandLine
{
    public static CommandLine Parse(string[] args)
    {
        CommandLine cli = new()
        {
            FieldCount = GetFieldCount(args)
        };

        (cli.OriginalPath, cli.PackagePath, cli.SourcePath) = GetPaths(args);

        return cli;
    }

    public static (string, string?, string?) GetPaths(string[] args)
    {
        string originalPath = args[args.Length - 1];
        string? packagePath = null;
        string? sourcePath;

        if ((originalPath?.StartsWith("pack:") ?? false) && originalPath.Contains('|'))
        {
            string[] paths = originalPath.Split('|');

            packagePath = paths[0].Substring(5).TrimStart('/', '\\');
            sourcePath = paths[1].TrimStart('/', '\\');
        }
        else
        {
            sourcePath = originalPath;
        }

        return (originalPath!, packagePath, sourcePath);
    }

    public static int? GetFieldCount(string[] args)
    {
        int? fieldCount = null;
        fieldCount = args.Any(arg => arg.Equals("/v0")) ? 0 : fieldCount;
        fieldCount = args.Any(arg => arg.Equals("/v1")) ? 1 : fieldCount;
        fieldCount = args.Any(arg => arg.Equals("/v2")) ? 2 : fieldCount;
        fieldCount = args.Any(arg => arg.Equals("/v3")) ? 3 : fieldCount;
        fieldCount = args.Any(arg => arg.Equals("/v4")) ? null : fieldCount;
        return fieldCount;
    }
}
