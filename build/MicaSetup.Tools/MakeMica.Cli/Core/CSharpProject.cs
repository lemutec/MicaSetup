using MakeMica.Shared;
using System.Xml.Linq;

namespace MakeMica.Cli.Core;

public static class CSharpProject
{
    public static void SetupConfig(string csprojPath, MicaConfig config, bool isUninst = false)
    {
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

        if (!isUninst && (!string.IsNullOrWhiteSpace(config.LicenseFile) || !string.IsNullOrWhiteSpace(config.License)))
        {
            // A custom license replaces the built-in per-language files with one license.txt or license.rtf.
            string licenseExt = string.Equals(config.LicenseType, "rtf", StringComparison.OrdinalIgnoreCase)
                ? "rtf"
                : "txt";
            UseSingleLicenseResource(doc, licenseExt);
        }

        doc.Save(csprojPath);
    }

    private static void UseSingleLicenseResource(XDocument doc, string licenseExt)
    {
        List<XElement> licenseResources = doc.Descendants("Resource")
            .Where(element => IsLicenseInclude((string?)element.Attribute("Include")))
            .ToList();

        if (licenseResources.Count == 0)
        {
            return;
        }

        licenseResources[0].AddBeforeSelf(new XElement("Resource",
            new XAttribute("Include", $@"Resources\Licenses\license.{licenseExt}")));

        foreach (XElement element in licenseResources)
        {
            element.Remove();
        }
    }

    private static bool IsLicenseInclude(string? include)
    {
        if (string.IsNullOrWhiteSpace(include))
        {
            return false;
        }

        return include.Contains(@"Resources\Licenses\license", StringComparison.OrdinalIgnoreCase)
            || include.Contains("Resources/Licenses/license", StringComparison.OrdinalIgnoreCase);
    }
}
