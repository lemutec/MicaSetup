using System.Runtime.InteropServices;

namespace MicaSetup.Win32;

public static class SHCore
{
    [DllImport(Lib.SHCore)]
    public static extern uint SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

    [DllImport(Lib.SHCore)]
    public static extern int GetDpiForMonitor(nint hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);
}
