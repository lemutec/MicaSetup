using PureSharpCompress.Archives;
using PureSharpCompress.Archives.GZip;
using PureSharpCompress.Archives.Rar;
using PureSharpCompress.Archives.SevenZip;
using PureSharpCompress.Archives.Zip;
using PureSharpCompress.Common;
using PureSharpCompress.Readers;

namespace MakeMica.Cli.Helper;

internal static class ArchiveFileHelper
{
    public static void ExtractAll(string destinationDirectory, string filePath, ReaderOptions? readerOptions = null!, ExtractionOptions? options = null)
    {
        using dynamic archive = filePath.OpenArchive(readerOptions);
        using IReader reader = archive.ExtractAllEntries();
        reader.WriteAllToDirectory(destinationDirectory, options);
    }

    public static void ExtractAll(string destinationDirectory, Stream stream, ReaderOptions? readerOptions = null!, ExtractionOptions? options = null)
    {
        using dynamic archive = stream.OpenArchive(readerOptions);
        using IReader reader = archive.ExtractAllEntries();
        long currentTotalSize = default;

        while (reader.MoveToNextEntry())
        {
            reader.WriteEntryToDirectory(destinationDirectory, options);
            currentTotalSize += reader.Entry.Size;
        }
    }

    public static IEnumerable<IArchiveEntry?> ReadStream(string filePath, string targetEntryKey, ReaderOptions? readerOptions = null!)
    {
        using dynamic archive = filePath.OpenArchive(readerOptions);
        targetEntryKey = targetEntryKey.NormalizeEntry();

        foreach (dynamic entry in archive.Entries)
        {
            string entryKey = ((string)entry.Key).NormalizeEntry();

            if (entryKey.Equals(targetEntryKey, StringComparison.OrdinalIgnoreCase))
            {
                // Use yield to ensure dispose of archive files.
                yield return entry as IArchiveEntry;
            }
        }

        yield break;
    }

    public static IEnumerable<IArchiveEntry?> ReadStream(string targetEntryKey, Stream stream, ReaderOptions? readerOptions = null!)
    {
        using dynamic archive = stream.OpenArchive(readerOptions);
        targetEntryKey = targetEntryKey.NormalizeEntry();

        foreach (dynamic entry in archive.Entries)
        {
            string entryKey = ((string)entry.Key).NormalizeEntry();

            if (entryKey.Equals(targetEntryKey, StringComparison.OrdinalIgnoreCase))
            {
                yield return entry as IArchiveEntry;
            }
        }

        yield break;
    }
}

file enum ArchiveFileType
{
    Unknown,
    SevenZip,
    Zip,
    Rar,
    GZip,
}

file static class ArchiveFileHelperExtension
{
    public static ArchiveFileType GetArchiveType(this Stream stream)
    {
        byte[] header = new byte[3];
        stream.Read(header, 0, header.Length);

        if (header[0] == 0x37 && header[1] == 0x7A)
        {
            return ArchiveFileType.SevenZip;
        }
        else if (header[0] == 0x50 && header[1] == 0x4B)
        {
            return ArchiveFileType.Zip;
        }
        else if (header[0] == 0x52 && header[1] == 0x61 && header[2] == 0x72)
        {
            return ArchiveFileType.Rar;
        }
        else if (header[0] == 0x1F && header[1] == 0x8B)
        {
            return ArchiveFileType.GZip;
        }
        return ArchiveFileType.Unknown;
    }

    public static ArchiveFileType GetArchiveType(this string filePath)
    {
        using Stream fileStream = filePath.OpenRead();
        return GetArchiveType(fileStream);
    }

    public static Stream OpenRead(this string filePath)
    {
        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public static dynamic OpenArchive(this string filePath, ReaderOptions? readerOptions = null!)
    {
        ArchiveFileType type = filePath.GetArchiveType();
        dynamic? archive = type switch
        {
            ArchiveFileType.Zip => ZipArchive.Open(filePath, readerOptions),
            ArchiveFileType.GZip => GZipArchive.Open(filePath, readerOptions),
            ArchiveFileType.Rar => RarArchive.Open(filePath, readerOptions),
            ArchiveFileType.SevenZip or _ => SevenZipArchive.Open(filePath, readerOptions),
        };
        return archive;
    }

    public static dynamic OpenArchive(this Stream stream, ReaderOptions? readerOptions = null!)
    {
        ArchiveFileType type = stream.GetArchiveType();
        using dynamic? archive = type switch
        {
            ArchiveFileType.Zip => ZipArchive.Open(stream, readerOptions),
            ArchiveFileType.GZip => GZipArchive.Open(stream, readerOptions),
            ArchiveFileType.Rar => RarArchive.Open(stream, readerOptions),
            ArchiveFileType.SevenZip or _ => SevenZipArchive.Open(stream, readerOptions),
        };
        return archive;
    }

    public static string NormalizeEntry(this string path)
    {
        if (path.StartsWith("./") || path.StartsWith(".\\"))
        {
            path = path.Substring(2);
        }

        return path.Replace("\\", "/");
    }
}
