using System.Diagnostics;
using System.IO;

namespace MICA;

internal sealed class Program
{
    private static void Main()
    {
        try
        {
            string currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            if (!string.IsNullOrEmpty(currentDirectory))
            {
                using Process Process = Process.Start(new ProcessStartInfo
                {
                    FileName = currentDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "open",
                });
            }
        }
        catch
        {
            ///
        }
    }
}
