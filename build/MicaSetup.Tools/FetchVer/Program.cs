using System.Reflection;

namespace FetchVer;

internal sealed class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"Startup: fetchver v{Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");

        if (args.Length <= 0)
        {
            // Support exe/dll/csproj/cs/7z/zip, etc files
            Console.WriteLine("Usage: fetchver [option] \"path/to/exe\"");

#if DEBUG
            args = ["FetchVer.exe"];
#endif
        }
    }
}
