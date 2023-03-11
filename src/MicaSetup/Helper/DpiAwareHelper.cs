using MicaSetup.Core;

namespace MicaSetup.Win32;

public static class DpiAwareHelper
{
    public static bool SetProcessDpiAwareness()
    {
        if (OsHelper.IsWindows81_OrGreater)
        {
            if (SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE) == 0)
            {
                return true;
            }
        }
        return false;
    }
}
