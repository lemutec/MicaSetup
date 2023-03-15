using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialogs;

[Flags]
internal enum SetWindowPosFlags
{
    SWP_NOSIZE = 0x0001,
    SWP_NOMOVE = 0x0002,
    SWP_NOZORDER = 0x0004,
    SWP_NOREDRAW = 0x0008,
    SWP_NOACTIVATE = 0x0010,
    SWP_FRAMECHANGED = 0x0020,
    SWP_SHOWWINDOW = 0x0040,
    SWP_HIDEWINDOW = 0x0080,
    SWP_NOCOPYBITS = 0x0100,
    SWP_NOOWNERZORDER = 0x0200,
    SWP_NOSENDCHANGING = 0x0400,
    SWP_DRAWFRAME = 0x0020,
    SWP_NOREPOSITION = 0x0200,
    SWP_DEFERERASE = 0x2000,
    SWP_ASYNCWINDOWPOS = 0x4000,
}

internal enum WindowLongFlags
{
    GWL_EXSTYLE = -20,
    GWLP_HINSTANCE = -6,
    GWLP_HWNDPARENT = -8,
    GWL_ID = -12,
    GWL_STYLE = -16,
    GWL_USERDATA = -21,
    GWL_WNDPROC = -4,
    DWLP_USER = 0x8,
    DWLP_MSGRESULT = 0x0,
    DWLP_DLGPROC = 0x4,
}

internal static class WindowNativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, SetWindowPosFlags flags);
}
