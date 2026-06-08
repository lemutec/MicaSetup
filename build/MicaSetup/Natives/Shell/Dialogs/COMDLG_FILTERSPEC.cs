using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
internal struct COMDLG_FILTERSPEC
{
    [MarshalAs(UnmanagedType.LPWStr)]
    public string? PszName;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string? PszSpec;
}
