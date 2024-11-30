using MakeMica.Cli.PEParser.PEImageParser;
using MakeMica.Cli.PEParser.PERsrcParser;
using PureSharpCompress.Archives;

namespace MakeMica.Cli.Helper;

internal static class PEImageHelper
{
    public static string? GetFileVersionFromArchive(string filePath, string targetEntryKey)
    {
        foreach (IArchiveEntry? entry in ArchiveFileHelper.ReadStream(filePath, targetEntryKey))
        {
            if (entry != null)
            {
                using Stream stream = entry.OpenEntryStream();
                using BinaryReader binaryReader = new(stream);
                byte[] byteArray = binaryReader.ReadBytes(1024);
                PEImage peImage = PEImage.FromBinary(byteArray);
                ImageSection imageSection = peImage.Sections.Where(section => section.Header.Name == ".rsrc").First();
                imageSection.SetDataFromRsrc(stream, (uint)entry.Size);
                string? fileVersion = RsrcParser.GetFileVersionFromRsrc(imageSection.Data);
                return fileVersion;
            }
        }
        return null;
    }
}
