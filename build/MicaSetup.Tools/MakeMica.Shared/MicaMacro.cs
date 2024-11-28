using System.IO;
using System.Reflection;

namespace MakeMica.Shared;

public static class MicaMacro
{
    public const string MicaDir = "${MicaDir}";
    public const string AppName = "${AppName}";
    public const string Version = "${Version}";

    public static string GetMicaDir()
        => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
}
