using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[ComImport,
Guid(IIDGuid.IFileDialog),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IFileDialog
{
    [PreserveSig]
    public int Show([In] nint parent);

    public void SetFileTypes([In] uint cFileTypes, [In][MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

    public void SetFileTypeIndex([In] uint iFileType);

    public void GetFileTypeIndex(out uint piFileType);

    public void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

    public void Unadvise([In] uint dwCookie);

    public void SetOptions([In] FOS fos);

    public void GetOptions(out FOS pfos);

    public void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

    public void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

    public void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

    public void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

    public void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

    public void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

    public void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    public void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, int alignment);

    public void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

    public void Close([MarshalAs(UnmanagedType.Error)] int hr);

    public void SetClientGuid([In] ref Guid guid);

    public void ClearClientData();

    public void SetFilter([MarshalAs(UnmanagedType.Interface)] nint pFilter);
}
