using MicaSetup.Helper;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;

namespace MicaSetup.Services;

public class MuiLanguageService : IMuiLanguageService
{
    public FontFamily FontFamily { get; set; } = null!;

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

        if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en")
        {
            return GetUriString(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
        }

        throw new Exception($"[MuiLanguageService] NotFound with match mui lang name of '{CultureInfo.CurrentUICulture.Name}' or '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' or '{CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName}'.");
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

        if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en")
        {
            return GetUriString(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
        }

        throw new Exception($"[MuiLanguageService] NotFound with match mui license name of '{CultureInfo.CurrentUICulture.Name}' or '{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}' or '{CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName}'.");
    }

    public void DebugPrint()
    {
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        Logger.Debug("Display Name", "Name", "TwoName", "ThreeName");

        foreach (var culture in cultures)
        {
            if ((culture.CultureTypes & CultureTypes.UserCustomCulture) == CultureTypes.UserCustomCulture)
            {
                continue;
            }
            Logger.Debug(culture.DisplayName, culture.Name, culture.TwoLetterISOLanguageName, culture.ThreeLetterISOLanguageName);
            if (culture.DisplayName.Contains("__EDITME__"))
            {
                Debugger.Break();
            }
        }
    }
}
