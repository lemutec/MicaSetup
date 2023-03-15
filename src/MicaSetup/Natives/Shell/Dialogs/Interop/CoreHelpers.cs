using System;
using System.Globalization;
using System.Text;

namespace MicaSetup.Shell.Dialogs;

public static class CoreHelpers
{
    public static bool RunningOnVista => Environment.OSVersion.Version.Major >= 6;

    public static bool RunningOnWin7 =>
            Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0;

    public static bool RunningOnXP => Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.Major >= 5;

    public static string GetStringResource(string resourceId)
    {
        string[] parts;
        string library;
        int index;

        if (string.IsNullOrEmpty(resourceId)) { return string.Empty; }

        resourceId = resourceId.Replace("shell32,dll", "shell32.dll");
        parts = resourceId.Split(new char[] { ',' });

        library = parts[0];
        library = library.Replace(@"@", string.Empty);
        library = Environment.ExpandEnvironmentVariables(library);
        var handle = CoreNativeMethods.LoadLibrary(library);

        parts[1] = parts[1].Replace("-", string.Empty);
        index = int.Parse(parts[1], CultureInfo.InvariantCulture);

        var stringValue = new StringBuilder(255);
        var retval = CoreNativeMethods.LoadString(handle, index, stringValue, 255);

        return retval != 0 ? stringValue.ToString() : null!;
    }

    public static void ThrowIfNotVista()
    {
        if (!CoreHelpers.RunningOnVista)
        {
            throw new PlatformNotSupportedException(LocalizedMessages.CoreHelpersRunningOnVista);
        }
    }

    public static void ThrowIfNotWin7()
    {
        if (!CoreHelpers.RunningOnWin7)
        {
            throw new PlatformNotSupportedException(LocalizedMessages.CoreHelpersRunningOn7);
        }
    }

    public static void ThrowIfNotXP()
    {
        if (!CoreHelpers.RunningOnXP)
        {
            throw new PlatformNotSupportedException(LocalizedMessages.CoreHelpersRunningOnXp);
        }
    }
}
