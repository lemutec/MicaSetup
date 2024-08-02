using MakeMica.Shared;
using System.Xml.Linq;

namespace MakeMica.Cli.Core;

public static class CSharpProject
{
    public static void SetupConfig(string csprojPath, MicaConfig config, bool isUninst = false)
    {
        _ = isUninst;

        if (!File.Exists(csprojPath))
        {
            return;
        }

        var doc = XDocument.Load(csprojPath);

        if (!string.IsNullOrWhiteSpace(config.TargetFramework))
        {
            string targetFramework = config.TargetFramework;

            // Only support .NET Framework 4.7.2, 4.8, and 4.8.1
            if (targetFramework != "net472"
                && targetFramework != "net48"
                && targetFramework != "net481")
            {
                return;
            }

            doc.Element("Project")
               .Element("PropertyGroup")
               .SetElementValue("TargetFramework", targetFramework);
        }

        doc.Save(csprojPath);
    }
}
