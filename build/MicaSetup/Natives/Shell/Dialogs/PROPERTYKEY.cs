using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Shell.Dialog;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct PROPERTYKEY
{
    public Guid Fmtid;
    public uint Pid;
}
