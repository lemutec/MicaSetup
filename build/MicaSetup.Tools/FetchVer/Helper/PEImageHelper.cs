using PEParser.PEImageParser;
using PEParser.PERsrcParser;
using PureSharpCompress.Archives;

namespace FetchVer.Helper;

internal static class PEImageHelper
{
    public static string? GetFileVersionFromArchive(string filePath, string targetEntryKey)
    {
        foreach (IArchiveEntry? entry in ArchiveFileHelper.ReadStream(filePath, targetEntryKey))
        {
            if (entry != null)
            {
                // Extract PE file from archive file.
                using Stream stream = entry.OpenEntryStream();
                using BinaryReader binaryReader = new(stream);
                byte[] byteArray = binaryReader.ReadBytes(1024);

                // Read PE header from archive entry.
                PEImage peImage = PEImage.FromBinary(byteArray);

                // Search FileVersion from .rsrc section.
                ImageSection imageSection = peImage.Sections.Where(section => section.Header.Name == ".rsrc").First();
                imageSection.SetDataFromRsrc(stream, (uint)entry.Size);
                string? fileVersion = RsrcParser.GetFileVersionFromRsrc(imageSection.Data);

                return fileVersion;
            }
        }
        return null;
    }
}
