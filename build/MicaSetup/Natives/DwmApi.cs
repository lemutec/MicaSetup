using System.Runtime.InteropServices;

namespace MicaSetup.Natives;

public static class DwmApi
{
    [DllImport(Lib.DwmApi)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int DwmSetWindowAttribute(nint hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);
}
