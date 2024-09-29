using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace MakeIcon.Cli.Helpers;

public static class PrivateFontHelper
{
    public static PrivateFontCollection PrivateFontCollection = new();
    public static FontFamily FontFamily => PrivateFontCollection.Families[0];

    static PrivateFontHelper()
    {
        byte[] fontData = ManifestResourceHelper.GetBytes("MakeIcon.Cli.Resources.Fonts.HarmonyOS_Icons_Slim.ttf");
        nint fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
        Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
        PrivateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
        Marshal.FreeCoTaskMem(fontPtr);
    }
}
