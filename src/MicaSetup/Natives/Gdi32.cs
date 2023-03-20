using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Natives;

public static class Gdi32
{
    [DllImport(Lib.Gdi32, SetLastError = false, ExactSpelling = true)]
    public static extern int GetDeviceCaps(nint hdc, DeviceCap nIndex);

    [DllImport(Lib.Gdi32, SetLastError = false, CharSet = CharSet.Auto)]
    public static extern nint CreateDC([Optional] string pwszDriver, [Optional] string pwszDevice, [Optional] string pszPort, object pdm);
}
