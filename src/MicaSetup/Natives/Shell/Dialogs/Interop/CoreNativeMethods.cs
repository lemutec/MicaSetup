using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MicaSetup.Shell.Dialogs;

internal static class CoreNativeMethods
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, PreserveSig = false, SetLastError = true)]
    public static extern void PostMessage(
        nint windowHandle,
        WindowMessage message,
        nint wparam,
        nint lparam
    );

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyIcon(nint hIcon);

    [DllImport("kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    internal static extern nint LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern int LoadString(
        nint instanceHandle,
        int id,
        StringBuilder buffer,
        int bufferSize);
}
