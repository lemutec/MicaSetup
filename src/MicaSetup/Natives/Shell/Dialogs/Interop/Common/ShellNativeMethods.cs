using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MicaSetup.Shell.Dialogs;

internal static class ShellNativeMethods
{
    internal const int CommandLink = 0x0000000E;

    internal const uint GetNote = 0x0000160A;

    internal const uint GetNoteLength = 0x0000160B;

    internal const int InPlaceStringTruncated = 0x00401A0;

    internal const int MaxPath = 260;

    internal const uint SetNote = 0x00001609;

    internal const uint SetShield = 0x0000160C;

    internal enum ControlState
    {
        Inactive = 0x00000000,
        Enable = 0x00000001,
        Visible = 0x00000002,
    }

    internal enum DefaultSaveFolderType
    {
        Detect = 1,
        Private = 2,
        Public = 3,
    };

    internal enum FileDialogAddPlacement
    {
        Bottom = 0x00000000,
        Top = 0x00000001,
    }

    internal enum FileDialogEventOverwriteResponse
    {
        Default = 0x00000000,
        Accept = 0x00000001,
        Refuse = 0x00000002,
    }

    internal enum FileDialogEventShareViolationResponse
    {
        Default = 0x00000000,
        Accept = 0x00000001,
        Refuse = 0x00000002,
    }

    [Flags]
    internal enum FileOpenOptions
    {
        OverwritePrompt = 0x00000002,
        StrictFileTypes = 0x00000004,
        NoChangeDirectory = 0x00000008,
        PickFolders = 0x00000020,
        ForceFilesystem = 0x00000040,
        AllNonStorageItems = 0x00000080,
        NoValidate = 0x00000100,
        AllowMultiSelect = 0x00000200,
        PathMustExist = 0x00000800,
        FileMustExist = 0x00001000,
        CreatePrompt = 0x00002000,
        ShareAware = 0x00004000,
        NoReadOnlyReturn = 0x00008000,
        NoTestFileCreate = 0x00010000,
        HideMruPlaces = 0x00020000,
        HidePinnedPlaces = 0x00040000,
        NoDereferenceLinks = 0x00100000,
        DontAddToRecent = 0x02000000,
        ForceShowHidden = 0x10000000,
        DefaultNoMiniMode = 0x20000000,
    }

    [Flags]
    internal enum GetPropertyStoreOptions
    {
        Default = 0,
        HandlePropertiesOnly = 0x1,
        ReadWrite = 0x2,
        Temporary = 0x4,
        FastPropertiesOnly = 0x8,
        OpensLowItem = 0x10,
        DelayCreation = 0x20,
        BestEffort = 0x40,
        MaskValid = 0xff,
    }

    internal enum LibraryFolderFilter
    {
        ForceFileSystem = 1,
        StorageItems = 2,
        AllItems = 3,
    };

    internal enum LibraryManageDialogOptions
    {
        Default = 0,
        NonIndexableLocationWarning = 1,
    };

    [Flags]
    internal enum LibraryOptions
    {
        Default = 0,
        PinnedToNavigationPane = 0x1,
        MaskAll = 0x1,
    };

    internal enum LibrarySaveOptions
    {
        FailIfThere = 0,
        OverrideExisting = 1,
        MakeUniqueName = 2,
    };

    [Flags]
    internal enum ShellChangeNotifyEventSource
    {
        InterruptLevel = 0x0001,
        ShellLevel = 0x0002,
        RecursiveInterrupt = 0x1000,
        NewDelivery = 0x8000,
    }

    [Flags]
    internal enum ShellFileGetAttributesOptions
    {
        CanCopy = 0x00000001,
        CanMove = 0x00000002,
        CanLink = 0x00000004,
        Storage = 0x00000008,
        CanRename = 0x00000010,
        CanDelete = 0x00000020,
        HasPropertySheet = 0x00000040,
        DropTarget = 0x00000100,
        CapabilityMask = 0x00000177,
        System = 0x00001000,
        Encrypted = 0x00002000,
        IsSlow = 0x00004000,
        Ghosted = 0x00008000,
        Link = 0x00010000,
        Share = 0x00020000,
        ReadOnly = 0x00040000,
        Hidden = 0x00080000,
        DisplayAttributeMask = 0x000FC000,
        FileSystemAncestor = 0x10000000,
        Folder = 0x20000000,
        FileSystem = 0x40000000,
        HasSubFolder = unchecked((int)0x80000000),
        ContentsMask = unchecked((int)0x80000000),
        Validate = 0x01000000,
        Removable = 0x02000000,
        Compressed = 0x04000000,
        Browsable = 0x08000000,
        Nonenumerated = 0x00100000,
        NewContent = 0x00200000,
        CanMoniker = 0x00400000,
        HasStorage = 0x00400000,
        Stream = 0x00400000,
        StorageAncestor = 0x00800000,
        StorageCapabilityMask = 0x70C50008,
        PkeyMask = unchecked((int)0x81044000),
    }

    [Flags]
    internal enum ShellFolderEnumerationOptions : ushort
    {
        CheckingForChildren = 0x0010,
        Folders = 0x0020,
        NonFolders = 0x0040,
        IncludeHidden = 0x0080,
        InitializeOnFirstNext = 0x0100,
        NetPrinterSearch = 0x0200,
        Shareable = 0x0400,
        Storage = 0x0800,
        NavigationEnum = 0x1000,
        FastItems = 0x2000,
        FlatList = 0x4000,
        EnableAsync = 0x8000,
    }

    internal enum ShellItemAttributeOptions
    {
        And = 0x00000001,
        Or = 0x00000002,
        AppCompat = 0x00000003,
        Mask = 0x00000003,
        AllItems = 0x00004000,
    }

    internal enum ShellItemDesignNameOptions
    {
        Normal = 0x00000000,
        ParentRelativeParsing = unchecked((int)0x80018001),
        DesktopAbsoluteParsing = unchecked((int)0x80028000),
        ParentRelativeEditing = unchecked((int)0x80031001),
        DesktopAbsoluteEditing = unchecked((int)0x8004c000),
        FileSystemPath = unchecked((int)0x80058000),
        Url = unchecked((int)0x80068000),
        ParentRelativeForAddressBar = unchecked((int)0x8007c001),
        ParentRelative = unchecked((int)0x80080001),
    }

    [Flags]
    internal enum SIIGBF
    {
        ResizeToFit = 0x00,
        BiggerSizeOk = 0x01,
        MemoryOnly = 0x02,
        IconOnly = 0x04,
        ThumbnailOnly = 0x08,
        InCacheOnly = 0x10,
    }

    [Flags]
    internal enum ThumbnailCacheOptions
    {
        Default = 0x00000000,
        LowQuality = 0x00000001,
        Cached = 0x00000002,
    }

    [Flags]
    internal enum ThumbnailOptions
    {
        Extract = 0x00000000,
        InCacheOnly = 0x00000001,
        FastExtract = 0x00000002,
        ForceExtraction = 0x00000004,
        SlowReclaim = 0x00000008,
        ExtractDoNotCache = 0x00000020
    }

    [DllImport("shell32.dll", CharSet = CharSet.None)]
    public static extern void ILFree(IntPtr pidl);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(IntPtr hObject);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint ILGetSize(IntPtr pidl);

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int PathParseIconLocation(
        [MarshalAs(UnmanagedType.LPWStr)] ref string pszIconFile);

    [DllImport("shell32.dll")]
    internal static extern IntPtr SHChangeNotification_Lock(
        IntPtr windowHandle,
        int processId,
        out IntPtr pidl,
        out uint lEvent);

    [DllImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SHChangeNotification_Unlock(IntPtr hLock);

    [DllImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SHChangeNotifyDeregister(uint hNotify);

    [DllImport("shell32.dll")]
    internal static extern uint SHChangeNotifyRegister(
        IntPtr windowHandle,
        ShellChangeNotifyEventSource sources,
        ShellObjectChangeTypes events,
        uint message,
        int entries,
        ref SHChangeNotifyEntry changeNotifyEntry);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHCreateItemFromIDList(
        /*PCIDLIST_ABSOLUTE*/ IntPtr pidl,
        ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem2 ppv);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHCreateItemFromParsingName(
        [MarshalAs(UnmanagedType.LPWStr)] string path,
        IntPtr pbc,
        ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHCreateItemFromParsingName(
        [MarshalAs(UnmanagedType.LPWStr)] string path,
        IntPtr pbc,
        ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHCreateShellItem(
        IntPtr pidlParent,
        [In, MarshalAs(UnmanagedType.Interface)] IShellFolder psfParent,
        IntPtr pidl,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi
    );

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHCreateShellItemArrayFromDataObject(
        System.Runtime.InteropServices.ComTypes.IDataObject pdo,
        ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItemArray iShellItemArray);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHGetDesktopFolder(
        [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf
    );

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHGetIDListFromObject(IntPtr iUnknown,
        out IntPtr ppidl
    );

    [DllImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SHParseDisplayName(
        [MarshalAs(UnmanagedType.LPWStr)] string pszName,
        IntPtr pbc,
        out IntPtr ppidl,
        ShellFileGetAttributesOptions sfgaoIn,
        out ShellFileGetAttributesOptions psfgaoOut
    );

    [DllImport("Shell32", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    internal static extern int SHShowManageLibraryUI(
        [In, MarshalAs(UnmanagedType.Interface)] IShellItem library,
        [In] IntPtr hwndOwner,
        [In] string title,
        [In] string instruction,
        [In] LibraryManageDialogOptions lmdOptions);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct FilterSpec
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Name;

        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Spec;

        internal FilterSpec(string name, string spec)
        {
            Name = name;
            Spec = spec;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SHChangeNotifyEntry
    {
        internal IntPtr pIdl;

        [MarshalAs(UnmanagedType.Bool)]
        internal bool recursively;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShellNotifyStruct
    {
        internal IntPtr item1;
        internal IntPtr item2;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct ThumbnailId
    {
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 16)]
        private readonly byte rgbKey;
    }
}
