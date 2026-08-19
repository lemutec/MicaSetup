using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml;

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

var command = args[0];
if (command is "-h" or "--help" or "help")
{
    PrintUsage();
    return 0;
}

if (!string.Equals(command, "add", StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine($"Unknown command: {command}");
    PrintUsage();
    return 1;
}

if (args.Length < 2)
{
    Console.Error.WriteLine("Missing package file path.");
    PrintUsage();
    return 1;
}

var packagePath = Path.GetFullPath(args[1]);
if (!File.Exists(packagePath))
{
    Console.Error.WriteLine($"File not found: {packagePath}");
    return 1;
}

var ext = Path.GetExtension(packagePath);
if (!ext.Equals(".nupkg", StringComparison.OrdinalIgnoreCase) &&
    !ext.Equals(".snupkg", StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine("Only .nupkg or .snupkg files are supported.");
    return 1;
}

try
{
    var packagesRoot = NugetPathResolver.ResolveGlobalPackagesFolder();
    Console.WriteLine($"Global packages folder: {packagesRoot}");

    var result = PackageAdder.Add(packagePath, packagesRoot);
    Console.WriteLine($"Restored: {result.PackageId} {result.Version}");
    Console.WriteLine($"Target directory: {result.TargetDirectory}");
    Console.WriteLine($"Checksum file: {result.Sha512Path}");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed: {ex.Message}");
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("""
        pmc — Restore a NuGet package into the local global packages folder

        Usage:
          pmc add <path-to.nupkg|path-to.snupkg>

        Description:
          Locates the system .nuget global packages folder (NUGET_PACKAGES / NuGet.Config / default path),
          extracts the package contents, and writes a .sha512 checksum file for local restore.
        """);
}

/// <summary>
/// Resolves the NuGet global packages folder (globalPackagesFolder).
/// </summary>
internal static class NugetPathResolver
{
    public static string ResolveGlobalPackagesFolder()
    {
        var fromEnv = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
        if (!string.IsNullOrWhiteSpace(fromEnv))
            return Path.GetFullPath(ExpandPath(fromEnv.Trim()));

        foreach (var configPath in EnumerateNugetConfigPaths())
        {
            var folder = TryReadGlobalPackagesFolder(configPath);
            if (!string.IsNullOrWhiteSpace(folder))
                return Path.GetFullPath(ExpandPath(folder));
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages");
    }

    private static IEnumerable<string> EnumerateNugetConfigPaths()
    {
        // Walk up from the current directory looking for nuget.config / NuGet.Config
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            foreach (var name in new[] { "nuget.config", "NuGet.Config" })
            {
                var path = Path.Combine(dir.FullName, name);
                if (File.Exists(path))
                    yield return path;
            }

            dir = dir.Parent;
        }

        // User-level config
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (!string.IsNullOrEmpty(appData))
        {
            var userConfig = Path.Combine(appData, "NuGet", "NuGet.Config");
            if (File.Exists(userConfig))
                yield return userConfig;
        }

        // Machine-level config (Windows)
        var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        if (!string.IsNullOrEmpty(programData))
        {
            var machineConfig = Path.Combine(programData, "NuGet", "NuGet.Config");
            if (File.Exists(machineConfig))
                yield return machineConfig;
        }
    }

    private static string? TryReadGlobalPackagesFolder(string configPath)
    {
        try
        {
            using var reader = XmlReader.Create(configPath, new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Prohibit,
            });

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element ||
                    !reader.Name.Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var key = reader.GetAttribute("key");
                if (!string.Equals(key, "globalPackagesFolder", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(key, "globalPackagesPath", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var value = reader.GetAttribute("value");
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }
        }
        catch (XmlException)
        {
            // Ignore corrupt config and continue falling back
        }

        return null;
    }

    private static string ExpandPath(string path)
    {
        if (path.StartsWith("~/", StringComparison.Ordinal) ||
            path.StartsWith("~\\", StringComparison.Ordinal))
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                path[2..]);
        }

        return Environment.ExpandEnvironmentVariables(path);
    }
}

internal sealed record PackageIdentity(string Id, string Version);

internal static class NuspecReader
{
    public static PackageIdentity ReadFromPackage(string packagePath)
    {
        using var archive = ZipFile.OpenRead(packagePath);

        var nuspec = archive.Entries.FirstOrDefault(e =>
            e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase) &&
            !e.FullName.Contains('/', StringComparison.Ordinal) &&
            !e.FullName.Contains('\\', StringComparison.Ordinal));

        nuspec ??= archive.Entries.FirstOrDefault(e =>
            e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));

        if (nuspec is null)
            throw new InvalidDataException("No .nuspec found in the package.");

        using var stream = nuspec.Open();
        return ReadIdentity(stream);
    }

    private static PackageIdentity ReadIdentity(Stream nuspecStream)
    {
        using var reader = XmlReader.Create(nuspecStream, new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Prohibit,
        });

        string? id = null;
        string? version = null;

        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element ||
                !reader.LocalName.Equals("metadata", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            using var meta = reader.ReadSubtree();
            // ReadElementContentAsString leaves the reader on the next node;
            // do not call Read() unconditionally or version will be skipped
            var moved = meta.Read();
            while (moved)
            {
                if (meta.NodeType == XmlNodeType.Element)
                {
                    if (meta.LocalName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        id = meta.ReadElementContentAsString().Trim();
                        continue;
                    }

                    if (meta.LocalName.Equals("version", StringComparison.OrdinalIgnoreCase))
                    {
                        version = meta.ReadElementContentAsString().Trim();
                        continue;
                    }
                }

                if (id is not null && version is not null)
                    break;

                moved = meta.Read();
            }

            break;
        }

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(version))
            throw new InvalidDataException("Unable to read id/version from .nuspec.");

        return new PackageIdentity(id, version);
    }
}

internal sealed record AddResult(
    string PackageId,
    string Version,
    string TargetDirectory,
    string Sha512Path);

internal static class PackageAdder
{
    private static readonly HashSet<string> SkipRootEntries = new(StringComparer.OrdinalIgnoreCase)
    {
        "[Content_Types].xml",
        "_rels",
        "package",
    };

    public static AddResult Add(string packagePath, string packagesRoot)
    {
        var identity = NuspecReader.ReadFromPackage(packagePath);
        var idFolder = identity.Id.ToLowerInvariant();
        var versionFolder = identity.Version.ToLowerInvariant();
        var targetDir = Path.Combine(packagesRoot, idFolder, versionFolder);

        Directory.CreateDirectory(targetDir);

        var isSymbol = Path.GetExtension(packagePath)
            .Equals(".snupkg", StringComparison.OrdinalIgnoreCase);
        var packageFileName = $"{idFolder}.{versionFolder}{(isSymbol ? ".snupkg" : ".nupkg")}";
        var targetPackagePath = Path.Combine(targetDir, packageFileName);

        if (isSymbol)
        {
            // Symbol package: copy .snupkg and write checksum; do not overwrite existing package content
            File.Copy(packagePath, targetPackagePath, overwrite: true);
        }
        else
        {
            ExtractPackageContent(packagePath, targetDir);
            File.Copy(packagePath, targetPackagePath, overwrite: true);
            RenameNuspecIfNeeded(targetDir, idFolder);
        }

        var hash = Sha512Helper.ComputeFileHashBase64(targetPackagePath);
        Sha512Helper.WriteNugetSha512File(targetPackagePath, hash);

        if (!isSymbol)
            WriteMetadata(targetDir, hash);

        return new AddResult(
            identity.Id,
            identity.Version,
            targetDir,
            targetPackagePath + ".sha512");
    }

    private static void ExtractPackageContent(string packagePath, string targetDir)
    {
        using var archive = ZipFile.OpenRead(packagePath);

        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue; // directory entry

            var relative = entry.FullName.Replace('\\', '/');
            var root = relative.Split('/', 2)[0];
            if (SkipRootEntries.Contains(root))
                continue;

            var destPath = Path.GetFullPath(
                Path.Combine(targetDir, relative.Replace('/', Path.DirectorySeparatorChar)));

            if (!destPath.StartsWith(
                    Path.GetFullPath(targetDir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) &&
                !destPath.Equals(Path.GetFullPath(targetDir), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"Illegal package path: {entry.FullName}");
            }

            var destDir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            entry.ExtractToFile(destPath, overwrite: true);
        }
    }

    private static void RenameNuspecIfNeeded(string targetDir, string packageIdLower)
    {
        var expectedName = packageIdLower + ".nuspec";
        var expectedPath = Path.Combine(targetDir, expectedName);
        var nuspecs = Directory.GetFiles(targetDir, "*.nuspec", SearchOption.TopDirectoryOnly);
        if (nuspecs.Length == 0)
            return;

        var current = nuspecs[0];
        // Paths are case-insensitive on Windows; compare the file name itself
        if (!string.Equals(Path.GetFileName(current), expectedName, StringComparison.Ordinal))
            File.Move(current, expectedPath, overwrite: true);
    }

    private static void WriteMetadata(string targetDir, string contentHash)
    {
        var path = Path.Combine(targetDir, ".nupkg.metadata");
        var json =
            "{\n" +
            "  \"version\": 2,\n" +
            $"  \"contentHash\": \"{contentHash}\",\n" +
            "  \"source\": \"local\"\n" +
            "}";
        File.WriteAllText(path, json);
    }
}

/// <summary>
/// Computes SHA512 the way NuGet validates local packages (same as common .ps1 scripts: raw file bytes → Base64).
/// NuGet *.nupkg.sha512 files contain only the Base64 hash, with no file name.
/// </summary>
internal static class Sha512Helper
{
    public static string ComputeFileHashBase64(string filePath)
    {
        using var sha = SHA512.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = sha.ComputeHash(stream);
        return Convert.ToBase64String(hashBytes);
    }

    public static void WriteNugetSha512File(string packageFilePath, string hashBase64)
    {
        var shaPath = packageFilePath + ".sha512";
        // NuGet checksum file format: plain Base64, no newline, no file name
        File.WriteAllText(shaPath, hashBase64);
    }
}
