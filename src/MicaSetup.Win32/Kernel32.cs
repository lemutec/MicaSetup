using System.Runtime.InteropServices;
using System.Security;

namespace MicaSetup.Win32;

public static class Kernel32
{
    [SecurityCritical]
    [DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
}
