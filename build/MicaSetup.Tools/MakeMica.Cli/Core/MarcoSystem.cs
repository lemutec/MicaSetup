using System.Reflection;

namespace MakeMica.Cli.Core;

internal static class MarcoSystem
{
    public const string MicaDirMarco = "${MICADIR}";
    public static string MicaDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
}
