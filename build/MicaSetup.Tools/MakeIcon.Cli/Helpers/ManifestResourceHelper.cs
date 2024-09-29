using System.Reflection;

namespace MakeIcon.Cli.Helpers;

public static class ManifestResourceHelper
{
    public static Stream GetStream(string name, Assembly assembly = null!)
    {
        Stream stream = (assembly ?? Assembly.GetExecutingAssembly()).GetManifestResourceStream(name);
        return stream;
    }

    public static byte[] GetBytes(string name, Assembly assembly = null!)
    {
        using Stream stream = GetStream(name, assembly ?? Assembly.GetExecutingAssembly());
        using BinaryReader reader = new(stream);
        return reader.ReadBytes((int)stream.Length);
    }
}
