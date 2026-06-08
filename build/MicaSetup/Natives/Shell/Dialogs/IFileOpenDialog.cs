using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[ComImport()]
[Guid(IIDGuid.IFileOpenDialog)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IFileOpenDialog : IFileDialog
{
    [PreserveSig]
    public new int Show([In] nint parent);

    public void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);

    public new void SetFileTypeIndex([In] uint iFileType);

    public new void GetFileTypeIndex(out uint piFileType);

    public new void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

    public new void Unadvise([In] uint dwCookie);

    public new void SetOptions([In] FOS fos);

    public new void GetOptions(out FOS pfos);

    public new void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

    public new void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

    public new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public new void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

    public new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

    public new void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

    public new void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

    public new void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    public new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    // void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);
    public void AddPlace(); // incomplete signature

    public new void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

    public new void Close([MarshalAs(UnmanagedType.Error)] int hr);

    public new void SetClientGuid([In] ref Guid guid);

    public new void ClearClientData();

    public new void SetFilter([MarshalAs(UnmanagedType.Interface)] nint pFilter);

    public void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

    public void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
}
