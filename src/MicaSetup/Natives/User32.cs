using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace MicaSetup.Natives;

public static class User32
{
    [SecurityCritical]
    [DllImport(Lib.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern nint PostMessage(nint hWnd, int msg, int wParam, int lParam);

    [DllImport(Lib.User32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport(Lib.User32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [SecurityCritical]
    [DllImport(Lib.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out POINT pt);

    [SecurityCritical]
    [DllImport(Lib.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern nint GetDC(nint hWnd);

    [SecurityCritical]
    [DllImport(Lib.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int ReleaseDC(nint hWnd, nint hDC);

    [SecurityCritical]
    [DllImport(Lib.User32, SetLastError = false, ExactSpelling = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern nint MonitorFromWindow(nint hwnd, MonitorFlags dwFlags);
}
