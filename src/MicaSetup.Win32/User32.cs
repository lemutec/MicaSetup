using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MicaSetup.Win32;

public static class User32
{
    [SecurityCritical]
    [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    [SecurityCritical]
    [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool GetCursorPos(out POINT pt);
}
