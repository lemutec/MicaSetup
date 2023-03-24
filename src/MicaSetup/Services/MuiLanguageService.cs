using MicaSetup.Helper;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;

namespace MicaSetup.Services;

public class MuiLanguageService : IMuiLanguageService
{
    public FontFamily FontFamily { get; set; } = null!;

    static MuiLanguageService()
    {
        DebugPrintPrivate();
    }

    public FontFamily GetFontFamily()
    {
        if (FontFamily == null)
        {
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
            {
                FontFamily = new FontFamily("Yu Gothic UI");
            }
            else
            {
                static string GetUriString(string name = null!) => $"pack://application:,,,/MicaSetup;component/Resources/Fonts/{name ?? string.Empty}";

                if (ResourceHelper.HasResource(GetUriString("HarmonyOS_Sans_SC_Regular.ttf")))
                {
                    FontFamily = new FontFamily(new Uri(GetUriString()), "./HarmonyOS_Sans_SC_Regular.ttf#HarmonyOS Sans SC");
                }
            }
        }
        return FontFamily ??= new FontFamily();
    }

    public string GetXamlUriString()
    {
        static string GetUriString(string name) => $"pack://application:,,,/MicaSetup;component/Resources/Languages/{name}.xaml";

        if (ResourceHelper.HasResource(GetUriString(CultureInfo.CurrentUICulture.Name)))
        {
            return GetUriString(CultureInfo.CurrentUICulture.Name);
        }
        else
        {
            if (ResourceHelper.HasResource(GetUriString(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)))
            {
                return GetUriString(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
            else
            {
                if (ResourceHelper.HasResource(GetUriString(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName)))
                {
                    return GetUriString(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
                }
            }
        }

        Logger.Debug($"[MuiLanguageService] NotFound with match mui lang name of '{CultureInfo.CurrentUICulture.Name}' or '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' or '{CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName}'.");
        return GetUriString("en");
    }

    public string GetLicenseUriString()
    {
        static string GetUriString(string name) => $"pack://application:,,,/MicaSetup;component/Resources/Licenses/license.{name}.txt";

        if (ResourceHelper.HasResource(GetUriString(CultureInfo.CurrentUICulture.Name)))
        {
            return GetUriString(CultureInfo.CurrentUICulture.Name);
        }
        else
        {
            if (ResourceHelper.HasResource(GetUriString(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)))
            {
                return GetUriString(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
            else
            {
                if (ResourceHelper.HasResource(GetUriString(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName)))
                {
                    return GetUriString(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
                }
            }
        }
        Logger.Debug($"[MuiLanguageService] NotFound with match mui license name of '{CultureInfo.CurrentUICulture.Name}' or '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' or '{CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName}'.");
        return GetUriString("en");
    }

    [Conditional("DEBUG")]
    private static void DebugPrintPrivate()
    {
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        Logger.Debug("Display Name", "Name", "TwoName", "ThreeName", "IetfTag");

        foreach (var culture in cultures)
        {
            if ((culture.CultureTypes & CultureTypes.UserCustomCulture) == CultureTypes.UserCustomCulture)
            {
                continue;
            }
            Logger.Debug(culture.DisplayName, culture.Name, culture.TwoLetterISOLanguageName, culture.ThreeLetterISOLanguageName, culture.IetfLanguageTag);
            if (culture.DisplayName.Contains("__EDITME__"))
            {
                Debugger.Break();
            }
        }
    }
}

public class MuiLanguageFont
{
    public string? Name { get; set; }
    public string? TwoName { get; set; }
    public string? ThreeName { get; set; }

    public string? ResourceFamilyName { get; set; }
    public string? SystemFamilyName { get; set; }
}

public static class MuiLanguageFontExtension
{
    public static MuiLanguageFont OnNameOf(this MuiLanguageFont self, string name)
    {
        self.Name = name;
        self.TwoName = null!;
        self.ThreeName = null!;
        return self;
    }

    public static MuiLanguageFont OnTwoNameOf(this MuiLanguageFont self, string twoName)
    {
        self.Name = null!;
        self.TwoName = twoName;
        self.ThreeName = null!;
        return self;
    }

    public static MuiLanguageFont OnThreeNameOf(this MuiLanguageFont self, string threeName)
    {
        self.Name = null!;
        self.TwoName = null!;
        self.ThreeName = threeName;
        return self;
    }

    public static MuiLanguageFont ForResourceFont(this MuiLanguageFont self, string resourceFamilyName)
    {
        self.ResourceFamilyName = resourceFamilyName;
        return self;
    }

    public static MuiLanguageFont ForSystemFont(this MuiLanguageFont self, string systemFamilyName)
    {
        self.SystemFamilyName = systemFamilyName;
        return self;
    }
}
