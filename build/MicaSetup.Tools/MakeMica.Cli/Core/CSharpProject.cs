using System.Xml.Linq;

namespace MakeMica.Cli.Core;

public static class CSharpProject
{
    public static void SetTargetFramework(string csprojPath, string targetFramework)
    {
        if (!File.Exists(csprojPath))
        {
            return;
        }

        // Only support .NET Framework 4.7.2, 4.8, and 4.8.1
        if (targetFramework != "net472"
            && targetFramework != "net48"
            && targetFramework != "net481")
        {
            return;
        }

        var doc = XDocument.Load(csprojPath);

        doc.Element("Project")
           .Element("PropertyGroup")
           .SetElementValue("TargetFramework", targetFramework);

        doc.Save(csprojPath);
    }
}
