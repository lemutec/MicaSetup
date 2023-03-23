using MicaSetup.Helper;
using MicaSetup.Services;
using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace MicaSetup.Core;

public class MuiLanguage
{
    public static string SystemLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public static void SetupLanguage()
    {
        _ = SetLanguage();
    }

    public static bool SetLanguage() => SystemLanguage switch
    {
        "zh" => SetLanguage("zh"),
        "ja" => SetLanguage("ja"),
        "en" or _ => SetLanguage("en"),
    };

    public static bool SetLanguage(string name = "en")
    {
        try
        {
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (dictionary.Source != null && dictionary.Source.OriginalString.Equals($"/Resources/Languages/{name}.xaml", StringComparison.Ordinal))
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dictionary);
                    Application.Current.Resources.MergedDictionaries.Add(dictionary);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            _ = e;
        }
        return false;
    }

    public static string Mui(string key)
    {
        try
        {
            if (Application.Current == null)
            {
                return MuiBaml(key);
            }
            if (Application.Current!.FindResource(key) is string value)
            {
                return value;
            }
        }
        catch (Exception e)
        {
            _ = e;
        }
        return null!;
    }

    public static string Mui(string key, params object[] args)
    {
        return string.Format(Mui(key)?.ToString(), args);
    }

    private static string MuiBaml(string key)
    {
        try
        {
            using Stream resourceXaml = ResourceHelper.GetStream(new MuiLanguageService().GetXamlUriString());
            if (BamlHelper.LoadBaml(resourceXaml) is ResourceDictionary resourceDictionary)
            {
                return (resourceDictionary[key] as string)!;
            }
        }
        catch (Exception e)
        {
            _ = e;
        }
        return null!;
    }
}
