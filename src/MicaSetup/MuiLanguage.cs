using MicaSetup.Core;
using System;
using System.Globalization;
using System.IO;
using System.Text;
#if MUI_ZH || MUI_JP || MUI_EN
using System.Threading;
#endif
using System.Windows;

namespace MicaSetup;

public class MuiLanguage
{
    public static string SystemLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public static void SetupLanguage()
    {
#if DEBUG
#if MUI_ZH
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-cn");
#elif MUI_JP
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("jp");
#elif MUI_EN
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
#endif
#endif
        _ = SetLanguage();
    }

    public static string GetLanguage() => SystemLanguage switch
    {
        "zh" => "zh",
        "jp" => "jp",
        "en" or _ => "en",
    };

    public static string GetLanguageXaml() => SystemLanguage switch
    {
        "zh" => "zh-cn",
        "jp" => "jp",
        "en" or _ => "en-us",
    };

    public static bool SetLanguage() => SystemLanguage switch
    {
        "zh" => SetLanguage("zh-cn"),
        "jp" => SetLanguage("jp"),
        "en" or _ => SetLanguage("en-us"),
    };

    public static bool SetLanguage(string name = "en-us")
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
            using Stream resourceXaml = ResourceHelper.GetStream($"pack://application:,,,/MicaSetup;component/Resources/Languages/{GetLanguageXaml()}.xaml");
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
