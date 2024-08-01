namespace MakeMica.Cli.Core;

internal sealed class MicaPath
{
    public static string GetFullPath(string? path)
    {
        string? newPath = path?.Replace(MarcoSystem.MicaDirMarco, MarcoSystem.MicaDir);

        if (string.IsNullOrWhiteSpace(newPath))
        {
            return newPath!;
        }
        return Path.GetFullPath(path);
    }
}
