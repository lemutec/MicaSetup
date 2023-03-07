using System.IO;
using System.Windows.Baml2006;
using System.Xaml;

namespace MicaSetup.Core;

public static class BamlHelper
{
    public static object LoadBaml(Stream stream)
    {
        Baml2006Reader reader = new (stream);
        XamlObjectWriter writer = new(reader.SchemaContext);

        while (reader.Read())
        {
            writer.WriteNode(reader);
        }
        return writer.Result;
    }
}
