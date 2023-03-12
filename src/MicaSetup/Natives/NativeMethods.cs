using System.Runtime.InteropServices;

namespace MicaSetup.Natives;

public static class NativeMethods
{
    public static void HideAllWindowButton(nint hwnd)
    {
        _ = User32.SetWindowLong(hwnd, (int)WindowLongFlags.GWL_STYLE, User32.GetWindowLong(hwnd, (int)WindowLongFlags.GWL_STYLE) & ~(int)WindowStyles.WS_SYSMENU);
    }

    public static int SetWindowAttribute(nint hwnd, DWMWINDOWATTRIBUTE attribute, int parameter)
    {
        return DwmApi.DwmSetWindowAttribute(hwnd, attribute, ref parameter, Marshal.SizeOf<int>());
    }
}
