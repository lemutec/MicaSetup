using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MicaSquircle.Core;

internal static class PrivateFontHelper
{
    public static PrivateFontCollection PrivateFontCollection = new();
    public static FontFamily FontFamily => PrivateFontCollection.Families[0];

    static PrivateFontHelper()
    {
        string fontFilePath = Environment.CurrentDirectory + @"\..\..\..\..\..\..\src\MicaSetup\Resources\Fonts\IcoMoon\output\fonts\icomoon.ttf";
        byte[] fontData = File.ReadAllBytes(fontFilePath);
        nint fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
        Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
        PrivateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
        Marshal.FreeCoTaskMem(fontPtr);
    }
}
