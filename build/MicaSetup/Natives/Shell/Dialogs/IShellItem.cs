using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[ComImport,
Guid(IIDGuid.IShellItem),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItem
{
    public void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] nint pbc, [In] ref Guid bhid, [In] ref Guid riid, out nint ppv);

    public void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    public void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

    public void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
}
