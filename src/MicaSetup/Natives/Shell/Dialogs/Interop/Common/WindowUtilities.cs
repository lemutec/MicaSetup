using MicaSetup.Natives;
using System;

namespace MicaSetup.Shell.Dialogs;

[Flags]
internal enum WindowStyles
{
    Border = 0x00800000,
    Caption = 0x00C00000,
    Child = 0x40000000,
    ChildWindow = 0x40000000,
    ClipChildren = 0x02000000,
    ClipSiblings = 0x04000000,
    Disabled = 0x08000000,
    DialogFrame = 0x0040000,
    Group = 0x00020000,
    HorizontalScroll = 0x00100000,
    Iconic = 0x20000000,
    Maximize = 0x01000000,
    MaximizeBox = 0x00010000,
    Minimize = 0x20000000,
    MinimizeBox = 0x00020000,
    Overlapped = 0x00000000,
    Popup = unchecked((int)0x80000000),
    SizeBox = 0x00040000,
    SystemMenu = 0x00080000,
    Tabstop = 0x00010000,
    ThickFrame = 0x00040000,
    Tiled = 0x00000000,
    Visible = 0x10000000,
    VerticalScroll = 0x00200000,
    TiledWindowMask = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,
    PopupWindowMask = Popup | Border | SystemMenu,
    OverlappedWindowMask = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,
}

internal static class WindowUtilities
{
    internal static System.Drawing.Size GetNonClientArea(nint hwnd)
    {
        var c = new POINT();

        TabbedThumbnailNativeMethods.ClientToScreen(hwnd, ref c);

        var r = new RECT();

        TabbedThumbnailNativeMethods.GetWindowRect(hwnd, ref r);

        return new System.Drawing.Size(c.X - r.Left, c.Y - r.Top);
    }

    internal static System.Drawing.Point GetParentOffsetOfChild(nint hwnd, nint hwndParent)
    {
        var childScreenCoord = new POINT();

        TabbedThumbnailNativeMethods.ClientToScreen(hwnd, ref childScreenCoord);

        var parentScreenCoord = new POINT();

        TabbedThumbnailNativeMethods.ClientToScreen(hwndParent, ref parentScreenCoord);

        var offset = new System.Drawing.Point(
            childScreenCoord.X - parentScreenCoord.X,
            childScreenCoord.Y - parentScreenCoord.Y);

        return offset;
    }
}
