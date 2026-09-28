using CheckVer.Helper;
using System.Reflection;

namespace CheckVer;

internal sealed class Program
{
    private static int Main(string[] args)
    {
        if (WineHelper.IsRunningUnderWine())
        {
            Console.WriteLine("INF: Detected Wine environment.");
        }

        Console.WriteLine($"Startup: checkver v{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)}");

        if (args.Length <= 0 || args.Any(static arg => arg is "/?" or "/help" or "-h" or "--help" or "-?"))
        {
            Console.WriteLine("Usage: checkver \"path/to/exe\"");

#if DEBUG
            if (args.Length <= 0 && File.Exists("CheckVer.exe"))
            {
                args = ["CheckVer.exe"];
            }
            else
            {
                return 1;
            }
#else
            return 1;
#endif
        }

        return DependencyInspector.Run(args[args.Length - 1]);
    }
}
