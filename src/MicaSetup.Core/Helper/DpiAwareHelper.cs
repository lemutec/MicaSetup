using System;

namespace MicaSetup.Win32;

public static class DpiAwareHelper
{
    public static bool IsSupported => Environment.OSVersion.Version >= new Version(6, 3);

    public static bool SetProcessDpiAwareness()
    {
        if (IsSupported)
        {
            if (SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE) == 0)
            {
                return true;
            }
        }
        return false;
    }
}
