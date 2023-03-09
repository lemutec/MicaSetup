﻿using MicaSetup.Win32;
using System;

namespace MicaSetup.Core;

public static class OsHelper
{
    private static Version? _versionCache;
    private static readonly Version _osVersion = GetOSVersion();

    public static bool IsWindowsNT { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;
    public static bool IsWindows7 { get; } = IsWindowsNT && _osVersion == new Version(6, 1);
    public static bool IsWindows7_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 1);
    public static bool IsWindows8 { get; } = IsWindowsNT && _osVersion == new Version(6, 2);
    public static bool IsWindows8_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 2);
    public static bool IsWindows81 { get; } = IsWindowsNT && _osVersion == new Version(6, 3);
    public static bool IsWindows81_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 3);
    public static bool IsWindows10 { get; } = IsWindowsNT && _osVersion == new Version(10, 0);
    public static bool IsWindows10_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0);
    public static bool IsWindows10_1507 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 10240);
    public static bool IsWindows10_1507_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 10240);
    public static bool IsWindows10_1511 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 10586);
    public static bool IsWindows10_1511_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 10586);
    public static bool IsWindows10_1607 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 14393);
    public static bool IsWindows10_1607_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 14393);
    public static bool IsWindows10_1703 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 15063);
    public static bool IsWindows10_1703_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 15063);
    public static bool IsWindows10_1709 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 16299);
    public static bool IsWindows10_1709_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 16299);
    public static bool IsWindows10_1803 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 17134);
    public static bool IsWindows10_1803_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 17134);
    public static bool IsWindows10_1809 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 17763);
    public static bool IsWindows10_1809_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 17763);
    public static bool IsWindows10_1903 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 18362);
    public static bool IsWindows10_1903_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 18362);
    public static bool IsWindows10_1909 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 18363);
    public static bool IsWindows10_1909_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 18363);
    public static bool IsWindows10_2004 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 19041);
    public static bool IsWindows10_2004_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 19041);
    public static bool IsWindows10_2009 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 19042);
    public static bool IsWindows10_2009_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 19042);
    public static bool IsWindows10_21H1 { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 19043);
    public static bool IsWindows10_21H1_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 19043);
    public static bool IsWindows11 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 22000);
    public static bool IsWindows11_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 22000);
    public static bool IsWindows11_22523 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 22523);
    public static bool IsWindows11_22523_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 22523);

    public static Version GetOSVersion()
    {
        if (_versionCache is null)
        {
            if (NTdll.RtlGetVersion(out var osv) != 0)
            {
                throw new PlatformNotSupportedException("Setup can only run on Windows.");
            }

            _versionCache = new Version(osv.MajorVersion, osv.MinorVersion, osv.BuildNumber, osv.Revision);
        }
        return _versionCache;
    }
}