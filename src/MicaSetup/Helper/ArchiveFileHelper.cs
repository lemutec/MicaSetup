using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;

namespace MicaSetup.Helper;

public static class ArchiveFileHelper
{
    public static long TotalUncompressSize(string filePath, ReaderOptions? readerOptions = null!)
    {
        using SevenZipArchive archive = SevenZipArchive.Open(filePath, readerOptions);
        return archive.TotalUncompressSize;
    }

    public static long TotalUncompressSize(Stream stream, ReaderOptions? readerOptions = null!)
    {
        using SevenZipArchive archive = SevenZipArchive.Open(stream, readerOptions);
        return archive.TotalUncompressSize;
    }

    public static string TotalUncompressSizeString(Stream stream, ReaderOptions? readerOptions = null!)
    {
        using SevenZipArchive archive = SevenZipArchive.Open(stream, readerOptions);
        long totalUncompressSize = archive.TotalUncompressSize;

        if (totalUncompressSize >= 1073741824)
        {
            return string.Format("{0:0.##}GB", (double)totalUncompressSize / 1073741824);
        }
        return string.Format("{0:0.##}MB", (double)totalUncompressSize / 1048576);
    }

    public static void ExtractAll(string destinationDirectory, string filePath, ReaderOptions? readerOptions = null!, ExtractionOptions? options = null)
    {
        using SevenZipArchive archive = SevenZipArchive.Open(filePath, readerOptions);
        using IReader reader = archive.ExtractAllEntries();
        reader.WriteAllToDirectory(destinationDirectory, options);
    }

    public static void ExtractAll(string destinationDirectory, Stream stream, Action<double, string> progressCallback = null!, ReaderOptions? readerOptions = null!, ExtractionOptions? options = null)
    {
        using SevenZipArchive archive = SevenZipArchive.Open(stream, readerOptions);
        using IReader reader = archive.ExtractAllEntries();
        long currentTotalSize = 0;

        while (reader.MoveToNextEntry())
        {
            reader.WriteEntryToDirectory(destinationDirectory, options);
            currentTotalSize += reader.Entry.Size;
            progressCallback?.Invoke(Math.Min(currentTotalSize / (double)archive.TotalUncompressSize, 1d), reader.Entry.Key);
        }
    }
}
