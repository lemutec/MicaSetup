using System.Reflection;

namespace MakeIcon.Cli.Helpers;

public static class ResourceHelper
{
    public static Stream GetManifestResourceStream(string name, Assembly assembly = null!)
    {
        Stream stream = (assembly ?? Assembly.GetExecutingAssembly()).GetManifestResourceStream(name);
        return stream;
    }

    public static byte[] GetManifestResourceBytes(string name, Assembly assembly = null!)
    {
        using Stream stream = GetManifestResourceStream(name, assembly ?? Assembly.GetExecutingAssembly());
        using BinaryReader reader = new(stream);
        return reader.ReadBytes((int)stream.Length);
    }
}
