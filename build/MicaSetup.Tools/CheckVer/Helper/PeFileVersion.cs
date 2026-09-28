using System.Text;

namespace CheckVer.Helper;

internal static class PeFileVersion
{
    public static string? TryGetFileVersion(byte[] image)
    {
        string? text = TryGetFileVersionString(image);
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        return TryGetFixedFileVersion(image);
    }

    private static string? TryGetFileVersionString(byte[] image)
    {
        byte[] key = Encoding.Unicode.GetBytes("FileVersion\0");
        int searchFrom = 0;

        while (searchFrom <= image.Length - key.Length)
        {
            int found = IndexOf(image, key, searchFrom);
            if (found < 0)
            {
                return null;
            }

            int basePosition = found + key.Length;
            for (int padding = 0; padding <= 4; padding++)
            {
                string value = ReadUtf16Z(image, basePosition + padding).Trim().Trim('\0');
                if (LooksLikeVersion(value))
                {
                    return value;
                }
            }

            searchFrom = found + 2;
        }

        return null;
    }

    private static string? TryGetFixedFileVersion(byte[] image)
    {
        for (int i = 0; i + 16 <= image.Length; i += 4)
        {
            if (image[i] != 0xBD || image[i + 1] != 0x04 || image[i + 2] != 0xEF || image[i + 3] != 0xFE)
            {
                continue;
            }

            uint structureVersion = BitConverter.ToUInt32(image, i + 4);
            if (structureVersion != 0x00010000)
            {
                continue;
            }

            uint fileVersionMs = BitConverter.ToUInt32(image, i + 8);
            uint fileVersionLs = BitConverter.ToUInt32(image, i + 12);
            return $"{fileVersionMs >> 16}.{fileVersionMs & 0xFFFF}.{fileVersionLs >> 16}.{fileVersionLs & 0xFFFF}";
        }

        return null;
    }

    private static bool LooksLikeVersion(string value)
    {
        if (value.Length is 0 or > 80 || !value.Any(char.IsDigit))
        {
            return false;
        }

        foreach (char character in value)
        {
            if (!char.IsLetterOrDigit(character) && character is not ('.' or ',' or ' ' or '+' or '-' or '_' or '(' or ')'))
            {
                return false;
            }
        }

        return true;
    }

    private static string ReadUtf16Z(byte[] data, int offset)
    {
        if (offset < 0 || offset >= data.Length)
        {
            return string.Empty;
        }

        int end = offset;
        while (end + 1 < data.Length && (data[end] != 0 || data[end + 1] != 0))
        {
            end += 2;
        }

        if (end == offset)
        {
            return string.Empty;
        }

        return Encoding.Unicode.GetString(data, offset, end - offset);
    }

    private static int IndexOf(byte[] data, byte[] pattern, int start)
    {
        for (int i = start; i <= data.Length - pattern.Length; i++)
        {
            bool matched = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (data[i + j] != pattern[j])
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
            {
                return i;
            }
        }

        return -1;
    }
}
