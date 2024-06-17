using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace MakeMui.Core;

internal static class ResourceDictionaryParser
{
    public static ResDict ParseResourceDictionary(XElement element)
    {
        ResDict resourceDictionary = new();

        foreach (XElement child in element.Elements())
        {
            Res resource = new()
            {
                Key = (string)(child.Attribute("Key") ?? child.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}Key")),
                Value = child.Value,
                Type = child.Name.LocalName,
                Content = child,
            };

            resourceDictionary.Resources.Add(resource);
        }

        return resourceDictionary;
    }
}

[DebuggerDisplay("{ToString()}")]
internal sealed class ResDict
{
    public List<Res> Resources { get; set; } = [];

    public override string ToString()
    {
        StringBuilder sb = new();

        foreach (Res resource in Resources)
        {
            sb.AppendLine(resource.ToString());
        }
        return sb.ToString();
    }
}

[DebuggerDisplay("{ToString()}")]
internal sealed class Res
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public XElement Content { get; set; } = null!;

    public override string ToString()
    {
        return $"{Key}:{Value}";
    }
}
