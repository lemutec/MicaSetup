using MakeMica.Cli;
using MakeMica.Shared;
using Newtonsoft.Json;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("Usage: makemica \"path/to/micasetup.json\"");
            return;
        }

        MicaConfig config;

        try
        {
            if (File.Exists(args[0]))
            {
                string jsonString = File.ReadAllText(args[0]);

                Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(args[0]));
                Console.WriteLine($"INF: Change directory to '{Environment.CurrentDirectory}'.");

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    config = JsonConvert.DeserializeObject<MicaConfig>(jsonString)
                        ?? throw new ArgumentException($"File '{args[0]}' is empty.");
                }
                else
                {
                    throw new ArgumentException($"ERR: File '{args[0]}' is empty.");
                }
            }
            else
            {
                throw new ArgumentException($"ERR: File '{args[0]}' not found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERR: " + e.Message);
            return;
        }

        App app = new();
        app.Run(config);
    }
}
