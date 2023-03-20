using System.Runtime.InteropServices;
using System.Security;
using System.Text;

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

    [DllImport(Lib.User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(nint hIcon);

    [DllImport(Lib.User32, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int LoadString(nint instanceHandle, int id, StringBuilder buffer, int bufferSize);

    [DllImport(Lib.User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ClientToScreen(nint hwnd, ref POINT point);

    [DllImport(Lib.User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(nint hwnd, ref RECT rect);
}
