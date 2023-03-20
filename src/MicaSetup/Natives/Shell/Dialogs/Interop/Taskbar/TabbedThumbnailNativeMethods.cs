using MicaSetup.Natives;
using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialogs;

internal static class TabbedThumbnailNativeMethods
{
    internal const int DisplayFrame = 0x00000001;

    internal const int ForceIconicRepresentation = 7;
    internal const int HasIconicBitmap = 10;

    internal const uint MsgfltAdd = 1;
    internal const uint MsgfltRemove = 2;
    internal const int ScClose = 0xF060;
    internal const int ScMaximize = 0xF030;
    internal const int ScMinimize = 0xF020;
    internal const uint WaActive = 1;
    internal const uint WaClickActive = 2;
    internal const uint WmDwmSendIconicLivePreviewBitmap = 0x0326;
    internal const uint WmDwmSendIconicThumbnail = 0x0323;

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint ChangeWindowMessageFilter(uint message, uint dwFlag);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ClientToScreen(
        nint hwnd,
        ref POINT point);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmInvalidateIconicBitmaps(nint hwnd);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetIconicLivePreviewBitmap(
        nint hwnd,
        nint hbitmap,
        ref POINT ptClient,
        uint flags);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetIconicLivePreviewBitmap(
        nint hwnd, nint hbitmap, nint ptClient, uint flags);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetIconicThumbnail(
        nint hwnd, nint hbitmap, uint flags);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    internal static extern int DwmSetWindowAttribute(
        nint hwnd,
        //DWMWA_* values.
        uint dwAttributeToSet,
        nint pvAttributeValue,
        uint cbAttribute);

    internal static void EnableCustomWindowPreview(nint hwnd, bool enable)
    {
        var t = Marshal.AllocHGlobal(4);
        Marshal.WriteInt32(t, enable ? 1 : 0);

        try
        {
            var rc = DwmSetWindowAttribute(hwnd, HasIconicBitmap, t, 4);
            if (rc != 0)
            {
                throw Marshal.GetExceptionForHR(rc);
            }

            rc = DwmSetWindowAttribute(hwnd, ForceIconicRepresentation, t, 4);
            if (rc != 0)
            {
                throw Marshal.GetExceptionForHR(rc);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(t);
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetClientRect(nint hwnd, ref RECT rect);

    internal static bool GetClientSize(nint hwnd, out System.Drawing.Size size)
    {
        var rect = new RECT();
        if (!GetClientRect(hwnd, ref rect))
        {
            size = new System.Drawing.Size(-1, -1);
            return false;
        }
        size = new System.Drawing.Size(rect.Right, rect.Bottom);
        return true;
    }

    [DllImport("user32.dll")]
    internal static extern nint GetWindowDC(nint hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(nint hwnd, ref RECT rect);

    [DllImport("user32.dll")]
    internal static extern int ReleaseDC(nint hwnd, nint hdc);

    internal static void SetIconicThumbnail(nint hwnd, nint hBitmap)
    {
        var rc = DwmSetIconicThumbnail(
            hwnd,
            hBitmap,
            DisplayFrame);
        if (rc != 0)
        {
            throw Marshal.GetExceptionForHR(rc);
        }
    }

    internal static void SetPeekBitmap(nint hwnd, nint bitmap, bool displayFrame)
    {
        var rc = DwmSetIconicLivePreviewBitmap(
            hwnd,
            bitmap,
            0,
            displayFrame ? DisplayFrame : (uint)0);
        if (rc != 0)
        {
            throw Marshal.GetExceptionForHR(rc);
        }
    }

    internal static void SetPeekBitmap(nint hwnd, nint bitmap, System.Drawing.Point offset, bool displayFrame)
    {
        var nativePoint = new POINT(offset.X, offset.Y);
        var rc = DwmSetIconicLivePreviewBitmap(
            hwnd,
            bitmap,
            ref nativePoint,
            displayFrame ? DisplayFrame : (uint)0);

        if (rc != 0)
        {
            var e = Marshal.GetExceptionForHR(rc);

            if (e is ArgumentException)
            {
            }
            else
            {
                throw e;
            }
        }
    }

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool StretchBlt(
        nint hDestDC, int destX, int destY, int destWidth, int destHeight,
        nint hSrcDC, int srcX, int srcY, int srcWidth, int srcHeight,
        uint operation);
}
