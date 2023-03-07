using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Win32;

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;
}

[StructLayout(LayoutKind.Sequential)]
public struct OSVERSIONINFOEX
{
    public int OSVersionInfoSize;
    public int MajorVersion;
    public int MinorVersion;
    public int BuildNumber;
    public int Revision;
    public int PlatformId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string CSDVersion;
    public ushort ServicePackMajor;
    public ushort ServicePackMinor;
    public short SuiteMask;
    public byte ProductType;
    public byte Reserved;
}

[Flags]
public enum MoveFileFlags
{
    MOVEFILE_REPLACE_EXISTING = 0x1,
    MOVEFILE_COPY_ALLOWED = 0x2,
    MOVEFILE_DELAY_UNTIL_REBOOT = 0x4,
    MOVEFILE_WRITE_THROUGH = 0x8,
    MOVEFILE_CREATE_HARDLINK = 0x10,
    MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20
}
