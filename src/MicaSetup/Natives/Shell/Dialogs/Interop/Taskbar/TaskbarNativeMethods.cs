using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialogs;

public enum ThumbnailAlphaType
{
    Unknown = 0,

    NoAlphaChannel = 1,

    HasAlphaChannel = 2,
}

internal enum KnownDestinationCategory
{
    Frequent = 1,
    Recent
}

internal enum SetTabPropertiesOption
{
    None = 0x0,
    UseAppThumbnailAlways = 0x1,
    UseAppThumbnailWhenActive = 0x2,
    UseAppPeekAlways = 0x4,
    UseAppPeekWhenActive = 0x8
}

internal enum ShellAddToRecentDocs
{
    Pidl = 0x1,
    PathA = 0x2,
    PathW = 0x3,
    AppIdInfo = 0x4,
    AppIdInfoIdList = 0x5,
    Link = 0x6,
    AppIdInfoLink = 0x7,
}

internal enum TaskbarActiveTabSetting
{
    UseMdiThumbnail = 0x1,
    UseMdiLivePreview = 0x2
}

internal enum TaskbarProgressBarStatus
{
    NoProgress = 0,
    Indeterminate = 0x1,
    Normal = 0x2,
    Error = 0x4,
    Paused = 0x8
}

internal enum ThumbButtonMask
{
    Bitmap = 0x1,
    Icon = 0x2,
    Tooltip = 0x4,
    THB_FLAGS = 0x8
}

[Flags]
internal enum ThumbButtonOptions
{
    Enabled = 0x00000000,
    Disabled = 0x00000001,
    DismissOnClick = 0x00000002,
    NoBackground = 0x00000004,
    Hidden = 0x00000008,
    NonInteractive = 0x00000010
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct ThumbButton
{
    internal const int Clicked = 0x1800;

    [MarshalAs(UnmanagedType.U4)]
    internal ThumbButtonMask Mask;

    internal uint Id;
    internal uint Bitmap;
    internal nint Icon;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    internal string Tip;

    [MarshalAs(UnmanagedType.U4)]
    internal ThumbButtonOptions Flags;
}

internal static class TaskbarNativeMethods
{
    internal const int WmCommand = 0x0111;

    internal const uint WmDwmSendIconicLivePreviewBitmap = 0x0326;

    internal const uint WmDwmSendIconThumbnail = 0x0323;

    internal static readonly uint WmTaskbarButtonCreated = RegisterWindowMessage("TaskbarButtonCreated");

    [DllImport("shell32.dll")]
    public static extern int SHGetPropertyStoreForWindow(
        nint hwnd,
        ref Guid iid,
        [Out(), MarshalAs(UnmanagedType.Interface)]out IPropertyStore propertyStore);

    [DllImport("shell32.dll")]
    internal static extern void GetCurrentProcessExplicitAppUserModelID(
        [Out(), MarshalAs(UnmanagedType.LPWStr)] out string AppID);

    internal static IPropertyStore GetWindowPropertyStore(nint hwnd)
    {
        var guid = new Guid(ShellIIDGuid.IPropertyStore);
        var rc = SHGetPropertyStoreForWindow(
            hwnd,
            ref guid,
            out var propStore);
        if (rc != 0)
        {
            throw Marshal.GetExceptionForHR(rc);
        }
        return propStore;
    }

    [DllImport("user32.dll", EntryPoint = "RegisterWindowMessage", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

    [DllImport("shell32.dll")]
    internal static extern void SetCurrentProcessExplicitAppUserModelID(
        [MarshalAs(UnmanagedType.LPWStr)] string AppID);

    internal static void SetWindowProperty(nint hwnd, PropertyKey propkey, string value)
    {
        var propStore = GetWindowPropertyStore(hwnd);

        using (var pv = new PropVariant(value))
        {
            var result = propStore.SetValue(ref propkey, pv);
            if (!CoreErrorHelper.Succeeded(result))
            {
                throw new ShellException(result);
            }
        }

        Marshal.ReleaseComObject(propStore);
    }

    [DllImport("shell32.dll")]
    internal static extern void SHAddToRecentDocs(
        ShellAddToRecentDocs flags,
        [MarshalAs(UnmanagedType.LPWStr)] string path);

    internal static void SHAddToRecentDocs(string path) => SHAddToRecentDocs(ShellAddToRecentDocs.PathW, path);

    internal static class TaskbarGuids
    {
        internal static Guid IObjectArray = new Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9");
        internal static Guid IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
    }
}