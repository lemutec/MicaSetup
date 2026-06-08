using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[ComImport]
[Guid(IIDGuid.IShellItemArray)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IShellItemArray
{
    // Not supported: IBindCtx

    public void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] nint pbc, [In] ref Guid rbhid, [In] ref Guid riid, out nint ppvOut);

    public void GetPropertyStore([In] int flags, [In] ref Guid riid, out nint ppv);

    public void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out nint ppv);

    public void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

    public void GetCount(out uint pdwNumItems);

    public void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

    public void EnumItems([MarshalAs(UnmanagedType.Interface)] out nint ppenumShellItems);
}
