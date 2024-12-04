using System.Runtime.InteropServices;

namespace PEParser.PERsrcParser;

public class RsrcParser
{
    public static string? GetFileVersionFromRsrc(byte[] rsrcData)
    {
        VS_FIXEDFILEINFO? fileInfo = FindFixedFileInfoOffset(rsrcData);

        if (fileInfo == null)
        {
            return null;
        }

        uint major = (fileInfo.Value.dwFileVersionMS >> 16) & 0xFFFF;
        uint minor = fileInfo.Value.dwFileVersionMS & 0xFFFF;
        uint build = (fileInfo.Value.dwFileVersionLS >> 16) & 0xFFFF;
        uint revision = fileInfo.Value.dwFileVersionLS & 0xFFFF;

        return $"{major}.{minor}.{build}.{revision}";
    }

    private static VS_FIXEDFILEINFO? FindFixedFileInfoOffset(byte[] rsrcData)
    {
        for (int i = 0; i < rsrcData.Length - 4; i++)
        {
            uint currentValue = BitConverter.ToUInt32(rsrcData, i);

            // Search VS_FFI_SIGNATURE in VS_FIXEDFILEINFO.dwSignature
            if (currentValue == 0xFEEF04BD)
            {
                VS_FIXEDFILEINFO fileInfo = MemoryMarshal.Cast<byte, VS_FIXEDFILEINFO>(
                    new Span<byte>(rsrcData, i, Marshal.SizeOf<VS_FIXEDFILEINFO>())
                )[0];
                if (fileInfo.dwFileType == (uint)VFT.VFT_APP)
                {
                    return fileInfo;
                }
            }
        }

        // VS_FFI_SIGNATURE not found.
        return null;
    }
}
