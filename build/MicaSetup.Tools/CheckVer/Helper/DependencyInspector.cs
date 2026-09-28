using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace CheckVer.Helper;

internal static class DependencyInspector
{
    public static int Run(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Error: Path is empty.");
            return 2;
        }

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(path);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Error: " + exception.Message);
            return 2;
        }

        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Error: File not found: {fullPath}");
            return 2;
        }

        try
        {
            WriteReport(fullPath);
            return 0;
        }
        catch (BadImageFormatException)
        {
            Console.WriteLine($"Error: Not a valid PE file: {fullPath}");
            return 2;
        }
        catch (Exception exception)
        {
            Console.WriteLine("Error: " + exception.Message);
            return 2;
        }
    }

    private static void WriteReport(string fullPath)
    {
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fullPath);
        string directory = Path.GetDirectoryName(fullPath) ?? string.Empty;

        using FileStream stream = File.OpenRead(fullPath);
        using PEReader peReader = new(stream);
        if (peReader.PEHeaders.PEHeader == null)
        {
            throw new BadImageFormatException();
        }

        Console.WriteLine("File: " + fullPath);
        Console.WriteLine("FileVersion: " + Display(fileVersionInfo.FileVersion));

        if (peReader.HasMetadata)
        {
            MetadataReader reader = peReader.GetMetadataReader();
            if (reader.IsAssembly)
            {
                AssemblyDefinition definition = reader.GetAssemblyDefinition();
                Console.WriteLine("Assembly: " + FormatDefinition(reader, definition));
            }
            else
            {
                Console.WriteLine("Assembly: (none)");
            }
        }
        else
        {
            Console.WriteLine("Assembly: (none)");
        }

        Console.WriteLine();
        WriteAssemblyReferences(peReader, directory);
        Console.WriteLine();
        WriteImportedDlls(peReader, directory, delayLoad: false);
        List<string> delayImports = ReadImportedDlls(peReader, delayLoad: true);
        if (delayImports.Count > 0)
        {
            Console.WriteLine();
            WriteDllSection("Delay Imports:", delayImports, directory);
        }

        Console.WriteLine();
        WriteCostura(peReader);
    }

    private static void WriteAssemblyReferences(PEReader peReader, string directory)
    {
        Console.WriteLine("Assembly References:");

        if (!peReader.HasMetadata)
        {
            Console.WriteLine("  (none)");
            return;
        }

        MetadataReader reader = peReader.GetMetadataReader();
        List<(string Display, string Name)> references = new();
        foreach (AssemblyReferenceHandle handle in reader.AssemblyReferences)
        {
            AssemblyReference reference = reader.GetAssemblyReference(handle);
            string name = reader.GetString(reference.Name);
            string culture = reader.GetString(reference.Culture);
            byte[] keyOrToken = ReadBlob(reader, reference.PublicKeyOrToken);
            bool isFullPublicKey = (reference.Flags & AssemblyFlags.PublicKey) != 0;
            references.Add((FormatName(name, reference.Version, culture, keyOrToken, isFullPublicKey), name));
        }

        if (references.Count == 0)
        {
            Console.WriteLine("  (none)");
            return;
        }

        foreach ((string Display, string Name) reference in references
            .GroupBy(item => item.Display, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Display, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("  " + reference.Display);
            WriteSiblingFileVersion(directory, reference.Name);
        }
    }

    private static void WriteImportedDlls(PEReader peReader, string directory, bool delayLoad)
    {
        WriteDllSection(delayLoad ? "Delay Imports:" : "Native Imports:", ReadImportedDlls(peReader, delayLoad), directory);
    }

    private static void WriteDllSection(string title, IReadOnlyList<string> names, string directory)
    {
        Console.WriteLine(title);
        if (names.Count == 0)
        {
            Console.WriteLine("  (none)");
            return;
        }

        foreach (string name in names)
        {
            Console.WriteLine("  " + name);
            string candidate = Path.Combine(directory, name);
            if (!File.Exists(candidate))
            {
                continue;
            }

            string? fileVersion = FileVersionInfo.GetVersionInfo(candidate).FileVersion;
            if (!string.IsNullOrWhiteSpace(fileVersion))
            {
                Console.WriteLine("    FileVersion: " + fileVersion);
            }
        }
    }

    private static List<string> ReadImportedDlls(PEReader peReader, bool delayLoad)
    {
        PEHeader? header = peReader.PEHeaders.PEHeader;
        if (header == null)
        {
            return new List<string>();
        }

        DirectoryEntry directory = delayLoad ? header.DelayImportTableDirectory : header.ImportTableDirectory;
        if (directory.RelativeVirtualAddress == 0 || directory.Size == 0)
        {
            return new List<string>();
        }

        PEMemoryBlock block = peReader.GetSectionData(directory.RelativeVirtualAddress);
        if (block.Length < (delayLoad ? 32 : 20))
        {
            return new List<string>();
        }

        int readable = Math.Min(directory.Size, block.Length);
        BlobReader reader = block.GetReader(0, readable);
        List<string> names = new();

        if (delayLoad)
        {
            while (reader.RemainingBytes >= 32)
            {
                reader.ReadUInt32();
                uint nameRva = reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();

                if (nameRva == 0)
                {
                    break;
                }

                AddName(names, ReadAsciiZ(peReader, (int)nameRva));
            }
        }
        else
        {
            while (reader.RemainingBytes >= 20)
            {
                uint originalFirstThunk = reader.ReadUInt32();
                uint timeDateStamp = reader.ReadUInt32();
                uint forwarderChain = reader.ReadUInt32();
                uint nameRva = reader.ReadUInt32();
                uint firstThunk = reader.ReadUInt32();

                if (originalFirstThunk == 0 && timeDateStamp == 0 && forwarderChain == 0 && nameRva == 0 && firstThunk == 0)
                {
                    break;
                }

                if (nameRva == 0)
                {
                    continue;
                }

                AddName(names, ReadAsciiZ(peReader, (int)nameRva));
            }
        }

        names.Sort(StringComparer.OrdinalIgnoreCase);
        return names;
    }

    private static void AddName(List<string> names, string name)
    {
        if (name.Length == 0)
        {
            return;
        }

        if (names.Any(existing => existing.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        names.Add(name);
    }

    private static void WriteCostura(PEReader peReader)
    {
        Console.WriteLine("Costura:");

        if (!peReader.HasMetadata || peReader.PEHeaders.CorHeader == null)
        {
            Console.WriteLine("  (none)");
            return;
        }

        MetadataReader reader = peReader.GetMetadataReader();
        List<(string Name, List<string> Lines)> entries = new();

        foreach (ManifestResourceHandle handle in reader.ManifestResources)
        {
            ManifestResource resource = reader.GetManifestResource(handle);
            string name = reader.GetString(resource.Name);
            if (!IsCosturaAssemblyResource(name))
            {
                continue;
            }

            List<string> lines = new();
            try
            {
                if (!resource.Implementation.IsNil)
                {
                    lines.Add("    Error: Resource is linked from another file.");
                }
                else
                {
                    byte[] payload = ReadEmbeddedResource(peReader, resource);
                    if (name.EndsWith(".compressed", StringComparison.OrdinalIgnoreCase))
                    {
                        payload = Decompress(payload);
                    }

                    if (payload.Length < 2 || payload[0] != (byte)'M' || payload[1] != (byte)'Z')
                    {
                        lines.Add("    Error: Payload is not a PE image.");
                    }
                    else
                    {
                        string? assembly = TryGetAssemblyDisplay(payload);
                        string? fileVersion = PeFileVersion.TryGetFileVersion(payload);
                        if (assembly != null)
                        {
                            lines.Add("    Assembly: " + assembly);
                        }

                        if (!string.IsNullOrWhiteSpace(fileVersion))
                        {
                            lines.Add("    FileVersion: " + fileVersion);
                        }

                        if (assembly == null && string.IsNullOrWhiteSpace(fileVersion))
                        {
                            lines.Add("    Version: (none)");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                lines.Add("    Error: " + exception.Message);
            }

            entries.Add((name, lines));
        }

        if (entries.Count == 0)
        {
            Console.WriteLine("  (none)");
            return;
        }

        foreach ((string Name, List<string> Lines) entry in entries.OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("  " + entry.Name);
            foreach (string line in entry.Lines)
            {
                Console.WriteLine(line);
            }
        }
    }

    private static bool IsCosturaAssemblyResource(string name)
    {
        if (name.Equals("costura.metadata", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!name.StartsWith("costura.", StringComparison.OrdinalIgnoreCase) &&
            !name.StartsWith("costura_", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string logicalName = name;
        if (logicalName.EndsWith(".compressed", StringComparison.OrdinalIgnoreCase))
        {
            logicalName = logicalName.Substring(0, logicalName.Length - ".compressed".Length);
        }

        if (logicalName.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return logicalName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
            logicalName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] ReadEmbeddedResource(PEReader peReader, ManifestResource resource)
    {
        CorHeader? corHeader = peReader.PEHeaders.CorHeader;
        if (corHeader == null || corHeader.ResourcesDirectory.RelativeVirtualAddress == 0)
        {
            throw new InvalidDataException("CLI resource directory is empty.");
        }

        DirectoryEntry resources = corHeader.ResourcesDirectory;
        PEMemoryBlock block = peReader.GetSectionData(resources.RelativeVirtualAddress);
        int available = block.Length;
        if (resources.Size > 0)
        {
            available = Math.Min(available, resources.Size);
        }

        if (resource.Offset >= available)
        {
            throw new InvalidDataException("Embedded resource offset is outside the CLI resource directory.");
        }

        int start = (int)resource.Offset;
        BlobReader reader = block.GetReader(start, block.Length - start);
        if (reader.RemainingBytes < 4)
        {
            throw new InvalidDataException("Embedded resource is truncated.");
        }

        uint length = reader.ReadUInt32();
        if (length > reader.RemainingBytes)
        {
            throw new InvalidDataException("Embedded resource length is invalid.");
        }

        return reader.ReadBytes((int)length);
    }

    private static byte[] Decompress(byte[] compressed)
    {
        using MemoryStream input = new(compressed, writable: false);
        using DeflateStream deflate = new(input, CompressionMode.Decompress);
        using MemoryStream output = new();
        deflate.CopyTo(output);
        return output.ToArray();
    }

    private static string? TryGetAssemblyDisplay(byte[] image)
    {
        try
        {
            using MemoryStream stream = new(image, writable: false);
            using PEReader peReader = new(stream);
            if (!peReader.HasMetadata)
            {
                return null;
            }

            MetadataReader reader = peReader.GetMetadataReader();
            if (!reader.IsAssembly)
            {
                return null;
            }

            return FormatDefinition(reader, reader.GetAssemblyDefinition());
        }
        catch (BadImageFormatException)
        {
            return null;
        }
    }

    private static void WriteSiblingFileVersion(string directory, string assemblyName)
    {
        if (directory.Length == 0)
        {
            return;
        }

        string dllPath = Path.Combine(directory, assemblyName + ".dll");
        string exePath = Path.Combine(directory, assemblyName + ".exe");
        string? candidate = File.Exists(dllPath) ? dllPath : File.Exists(exePath) ? exePath : null;
        if (candidate == null)
        {
            return;
        }

        string? fileVersion = FileVersionInfo.GetVersionInfo(candidate).FileVersion;
        if (!string.IsNullOrWhiteSpace(fileVersion))
        {
            Console.WriteLine($"    FileVersion: {fileVersion} ({Path.GetFileName(candidate)})");
        }
    }

    private static string FormatDefinition(MetadataReader reader, AssemblyDefinition definition)
    {
        byte[] publicKey = ReadBlob(reader, definition.PublicKey);
        return FormatName(
            reader.GetString(definition.Name),
            definition.Version,
            reader.GetString(definition.Culture),
            publicKey,
            isFullPublicKey: publicKey.Length > 0);
    }

    private static string FormatName(string name, Version version, string culture, byte[] keyOrToken, bool isFullPublicKey)
    {
        if (string.IsNullOrEmpty(culture))
        {
            culture = "neutral";
        }

        string token = keyOrToken.Length == 0 ? "null" : ToPublicKeyToken(keyOrToken, isFullPublicKey);
        return $"{name}, Version={version}, Culture={culture}, PublicKeyToken={token}";
    }

    private static string ToPublicKeyToken(byte[] blob, bool isFullPublicKey)
    {
        byte[] token = blob;
        if (isFullPublicKey || blob.Length != 8)
        {
            using SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(blob);
            token = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                token[i] = hash[hash.Length - 1 - i];
            }
        }

        return string.Concat(token.Select(value => value.ToString("x2")));
    }

    private static byte[] ReadBlob(MetadataReader reader, BlobHandle handle)
    {
        if (handle.IsNil)
        {
            return [];
        }

        BlobReader blob = reader.GetBlobReader(handle);
        return blob.ReadBytes(blob.Length);
    }

    private static string ReadAsciiZ(PEReader peReader, int rva)
    {
        if (rva <= 0)
        {
            return string.Empty;
        }

        PEMemoryBlock block = peReader.GetSectionData(rva);
        if (block.Length == 0)
        {
            return string.Empty;
        }

        int length = Math.Min(block.Length, 520);
        BlobReader reader = block.GetReader(0, length);
        byte[] bytes = reader.ReadBytes(length);
        int end = Array.IndexOf(bytes, (byte)0);
        if (end < 0)
        {
            end = bytes.Length;
        }

        return System.Text.Encoding.ASCII.GetString(bytes, 0, end);
    }

    private static string Display(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "(none)" : value!;
    }
}
