using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[ComImport,
Guid(IIDGuid.IFileDialogEvents),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IFileDialogEvents
{
    // NOTE: some of these callbacks are cancelable - returning S_FALSE means that
    // the dialog should not proceed (e.g. with closing, changing folder); to
    // support this, we need to use the PreserveSig attribute to enable us to return
    // the proper HRESULT
    [PreserveSig]
    public int OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

    [PreserveSig]
    public int OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

    public void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

    public void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

    public void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_SHAREVIOLATION_RESPONSE pResponse);

    public void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

    public void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_OVERWRITE_RESPONSE pResponse);
}
