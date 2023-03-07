using System.Runtime.InteropServices;
using System.Security;

namespace MicaSetup.Win32;

public static class NTdll
{
    [SecurityCritical]
    [DllImport(ExternDll.NTdll, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int RtlGetVersion(out OSVERSIONINFOEX versionInfo);
}
