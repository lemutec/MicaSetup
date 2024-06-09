using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Resources;

namespace MakeIcon.Helpers;

internal static class PrivateFontHelper
{
    public static PrivateFontCollection PrivateFontCollection = new();
    public static FontFamily FontFamily => PrivateFontCollection.Families[0];

    static PrivateFontHelper()
    {
        if (!UriParser.IsKnownScheme("pack"))
            _ = PackUriHelper.UriSchemePack;

        byte[] fontData = GetBytes("pack://application:,,,/MakeIcon;component/Resources/Fonts/HarmonyOS_Icons_Slim.ttf");
        nint fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
        Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
        PrivateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
        Marshal.FreeCoTaskMem(fontPtr);
    }

    public static byte[] GetBytes(string uriString)
    {
        Uri uri = new(uriString);
        StreamResourceInfo info = Application.GetResourceStream(uri);
        using BinaryReader stream = new(info.Stream);
        return stream.ReadBytes((int)info.Stream.Length);
    }
}
