using System.Runtime.InteropServices;

namespace MicaSetup.Natives;

public static class Shell32
{
    [DllImport(Lib.Shell32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern void SHChangeNotify(SHCNE wEventId, SHCNF uFlags, nint dwItem1, nint dwItem2);
}
