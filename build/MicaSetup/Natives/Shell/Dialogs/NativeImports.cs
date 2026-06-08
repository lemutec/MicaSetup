using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

internal static class NativeImports
{
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint GetActiveWindow();

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint SHCreateItemFromParsingName(
        [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
        nint pbc,
        ref Guid riid,
        out nint ppv);
}
