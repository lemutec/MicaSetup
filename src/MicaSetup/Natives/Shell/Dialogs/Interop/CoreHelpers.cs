using System;
using System.Globalization;
using System.Text;

namespace MicaSetup.Shell.Dialogs;

public static class CoreHelpers
{
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
}
