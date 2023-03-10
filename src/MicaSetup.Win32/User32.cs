using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MicaSetup.Win32;

public static class User32
{
    [SecurityCritical]
    [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern nint PostMessage(nint hWnd, int msg, int wParam, int lParam);

    [DllImport(ExternDll.User32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport(ExternDll.User32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [SecurityCritical]
    [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool GetCursorPos(out POINT pt);

    [SecurityCritical]
    [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetDpiForWindow(nint hWnd);
}
