using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MicaSetup.Shell.Dialogs;

internal static class CoreNativeMethods
{
    internal const int DWMNCRP_DISABLED = 1;

    internal const int DWMNCRP_ENABLED = 2;

    internal const int DWMNCRP_USEWINDOWSTYLE = 0;

    internal const int DWMWA_NCRENDERING_ENABLED = 1;

    internal const int DWMWA_NCRENDERING_POLICY = 2;

    internal const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;

    internal const int EnterIdleMessage = 0x0121;

    internal const int FormatMessageFromSystem = 0x00001000;

    internal const uint ResultFailed = 0x80004005;

    internal const uint ResultFalse = 1;

    internal const uint ResultInvalidArgument = 0x80070057;

    internal const uint ResultNotFound = 0x80070490;

    internal const uint StatusAccessDenied = 0xC0000022;

    internal const int UserMessage = 0x0400;

    public delegate int WNDPROC(IntPtr hWnd,
                uint uMessage,
                IntPtr wParam,
                IntPtr lParam);

    public static int GetHiWord(long value, int size) => (short)(value >> size);

    public static int GetLoWord(long value) => (short)(value & 0xFFFF);

    [DllImport("user32.dll", CharSet = CharSet.Auto, PreserveSig = false, SetLastError = true)]
    public static extern void PostMessage(
        IntPtr windowHandle,
        WindowMessage message,
        IntPtr wparam,
        IntPtr lparam
    );

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessage(
        IntPtr windowHandle,
        WindowMessage message,
        IntPtr wparam,
        IntPtr lparam
    );

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessage(
        IntPtr windowHandle,
        uint message,
        IntPtr wparam,
        IntPtr lparam
    );

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessage(
       IntPtr windowHandle,
       uint message,
       IntPtr wparam,
       [MarshalAs(UnmanagedType.LPWStr)] string lparam);

    public static IntPtr SendMessage(
        IntPtr windowHandle,
        uint message,
        int wparam,
        string lparam) => SendMessage(windowHandle, message, (IntPtr)wparam, lparam);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SendMessage(
        IntPtr windowHandle,
        uint message,
        ref int wparam,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lparam);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(IntPtr graphicsObjectHandle);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "DestroyWindow", CallingConvention = CallingConvention.StdCall)]
    internal static extern int DestroyWindow(IntPtr handle);

    [DllImport("kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern int LoadString(
        IntPtr instanceHandle,
        int id,
        StringBuilder buffer,
        int bufferSize);

    [DllImport("Kernel32.dll", EntryPoint = "LocalFree")]
    internal static extern IntPtr LocalFree(ref Guid guid);

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        private int width;
        private int height;

        public int Width { get => width; set => width = value; }

        public int Height { get => height; set => height = value; }
    };
}
