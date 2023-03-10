using System.Runtime.InteropServices;

namespace MicaSetup.Win32;

public static class DwmApi
{
    [DllImport(ExternDll.DwmApi)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int DwmSetWindowAttribute(nint hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);
}
