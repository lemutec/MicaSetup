using System.Runtime.InteropServices;
using System.Security;

namespace MicaSetup.Natives;

public static class Kernel32
{
    [DllImport(Lib.Kernel32, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern nint LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);

    [SecurityCritical]
    [DllImport(Lib.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
}
