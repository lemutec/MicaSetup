using MicaSetup.Natives;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MicaSetup.Shell.Dialogs;

[StructLayout(LayoutKind.Sequential)]
public struct Message
{
    private readonly IntPtr windowHandle;
    private readonly uint msg;
    private readonly IntPtr wparam;
    private readonly IntPtr lparam;
    private readonly int time;
    private POINT point;

    internal Message(IntPtr windowHandle, uint msg, IntPtr wparam, IntPtr lparam, int time, POINT point)
        : this()
    {
        this.windowHandle = windowHandle;
        this.msg = msg;
        this.wparam = wparam;
        this.lparam = lparam;
        this.time = time;
        this.point = point;
    }

    public IntPtr LParam => lparam;

    public uint Msg => msg;

    public POINT Point => point;

    public int Time => time;

    public IntPtr WindowHandle => windowHandle;

    public IntPtr WParam => wparam;

    public static bool operator !=(Message first, Message second) => !(first == second);

    public static bool operator ==(Message first, Message second) => first.WindowHandle == second.WindowHandle
            && first.Msg == second.Msg
            && first.WParam == second.WParam
            && first.LParam == second.LParam
            && first.Time == second.Time
            && first.Point == second.Point;

    public override bool Equals(object obj) => (obj != null && obj is Message) ? this == (Message)obj : false;

    public override int GetHashCode()
    {
        var hash = WindowHandle.GetHashCode();
        hash = hash * 31 + Msg.GetHashCode();
        hash = hash * 31 + WParam.GetHashCode();
        hash = hash * 31 + LParam.GetHashCode();
        hash = hash * 31 + Time.GetHashCode();
        hash = hash * 31 + Point.GetHashCode();
        return hash;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WindowClassEx
{
    internal uint Size;
    internal uint Style;

    internal ShellObjectWatcherNativeMethods.WndProcDelegate WndProc;

    internal int ExtraClassBytes;
    internal int ExtraWindowBytes;
    internal IntPtr InstanceHandle;
    internal IntPtr IconHandle;
    internal IntPtr CursorHandle;
    internal IntPtr BackgroundBrushHandle;

    internal string MenuName;
    internal string ClassName;

    internal IntPtr SmallIconHandle;
}

internal static class ShellObjectWatcherNativeMethods
{
    public delegate int WndProcDelegate(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam);

    [DllImport("Ole32.dll")]
    public static extern HResult CreateBindCtx(
        int reserved,
        [Out] out IBindCtx bindCtx);

    [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
        int extendedStyle,
        [MarshalAs(UnmanagedType.LPWStr)]
        string className,
        [MarshalAs(UnmanagedType.LPWStr)]
        string windowName,
        int style,
        int x,
        int y,
        int width,
        int height,
        IntPtr parentHandle,
        IntPtr menuHandle,
        IntPtr instanceHandle,
        IntPtr additionalData);

    [DllImport("User32.dll")]
    public static extern int DefWindowProc(
        IntPtr hwnd,
        uint msg,
        IntPtr wparam,
        IntPtr lparam);

    [DllImport("User32.dll")]
    public static extern void DispatchMessage([In] ref Message message);

    [DllImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMessage(
        [Out] out Message message,
        IntPtr windowHandle,
        uint filterMinMessage,
        uint filterMaxMessage);

    [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint RegisterClassEx(
        ref WindowClassEx windowClass
        );
}
