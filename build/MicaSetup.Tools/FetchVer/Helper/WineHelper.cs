using System.Runtime.InteropServices;

namespace FetchVer.Helper;

internal static class WineHelper
{
    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern nint GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern nint GetProcAddress(nint hModule, string procName);

    /// <summary>
    /// Detects whether the current process is running under Wine
    /// by checking for the Wine-specific export in ntdll.dll.
    /// </summary>
    public static bool IsRunningUnderWine()
    {
        nint ntdll = GetModuleHandle("ntdll.dll");
        if (ntdll == IntPtr.Zero)
        {
            return false;
        }

        return GetProcAddress(ntdll, "wine_get_version") != IntPtr.Zero;
    }
}
