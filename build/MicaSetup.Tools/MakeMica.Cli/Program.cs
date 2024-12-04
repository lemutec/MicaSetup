using MakeMica.Cli;
using MakeMica.Shared;
using Newtonsoft.Json;
using System.Reflection;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine($"Startup: makemica v{Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");

        if (args.Length <= 0)
        {
            if (!File.Exists("micasetup.json"))
            {
                Console.WriteLine("Usage: makemica \"path/to/micasetup.json\"");
                Environment.ExitCode = -1;
                return;
            }

            args = ["micasetup.json"];
        }

        MicaConfig config;
        string path = args[0];

        try
        {
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);

                Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(path));
                Console.WriteLine($"INF: Change directory to '{Environment.CurrentDirectory}'.");

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    config = JsonConvert.DeserializeObject<MicaConfig>(jsonString)
                        ?? throw new ArgumentException($"File '{path}' is empty.");
                }
                else
                {
                    throw new ArgumentException($"File '{path}' is empty.");
                }
            }
            else
            {
                throw new ArgumentException($"File '{path}' not found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERR: " + e.Message);
            Environment.ExitCode = -2;
            return;
        }

        try
        {
            App app = new();
            app.Run(config);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERR: " + e.Message);
            Environment.ExitCode = -3;
        }
    }
}
