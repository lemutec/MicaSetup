using System.Reflection;

namespace FetchVer;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine($"Startup: fetchver v{Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");

            // Support exe/dll/csproj/cs/7z/zip, etc files
            Console.WriteLine("Usage: fetchver [option] \"path/to/exe/or/archive/file\"");

#if DEBUG
            args = ["FetchVer.exe"];
#else
            return;
#endif
        }

        CommandLine cli = ParseCommandLine.Parse(args);
        cli.Execute();
    }
}
