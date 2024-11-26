using MicaSetup.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;

namespace MicaSetup.Core;

public static class Locale
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/typography/fonts/windows_11_font_list
    /// https://learn.microsoft.com/en-us/typography/fonts/windows_10_font_list
    /// https://learn.microsoft.com/en-us/typography/fonts/windows_81_font_list
    /// https://learn.microsoft.com/en-us/typography/fonts/windows_8_font_list
    /// https://learn.microsoft.com/en-us/typography/fonts/windows_7_font_list
    /// </summary>
    public static List<TrFont> FontSelector { get; } = [];

    public static event EventHandler? CultureChanged;

    public static CultureInfo Fallback { get; } = new CultureInfo("en-US");

    public static CultureInfo Culture
    {
        get => CultureInfo.CurrentUICulture;
        set => SetCulture(value);
    }

    private static void SetCulture(CultureInfo? value)
    {
        CultureInfo culture = Resolve(value);

        _ = SetCulture(Resolve(value).Name);

        CultureInfo.CurrentCulture
            = CultureInfo.CurrentUICulture
            = culture;

        CultureChanged?.Invoke(CultureChanged.Target, EventArgs.Empty);

        static bool SetCulture(string name)
        {
            if (Application.Current == null)
            {
                return false;
            }

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
    }

    public static CultureInfo Resolve(CultureInfo? value)
    {
        CultureInfo culture = value ?? Fallback;

        while (!HasCulture(culture))
        {
            if (culture.Parent == CultureInfo.InvariantCulture)
            {
                culture = Fallback;
                break;
            }
            culture = culture.Parent;
        }
        return culture;
    }

    public static bool HasCulture(CultureInfo culture)
        => ResourceHelper.HasResource($"pack://application:,,,/MicaSetup;component/Resources/Languages/{culture.Name}.xaml");
}

internal static class LocaleExtension
{
    public static string Tr(this string key)
    {
        try
        {
            if (Application.Current == null)
            {
                return TrBaml(key);
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

        static string TrBaml(string key)
        {
            try
            {
                using Stream resourceXaml = ResourceHelper.GetStream($"pack://application:,,,/MicaSetup;component/Resources/Languages/{Locale.Resolve(CultureInfo.CurrentUICulture).Name}.xaml");
                if (LoadBaml(resourceXaml) is ResourceDictionary resourceDictionary)
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

        static object LoadBaml(Stream stream)
        {
            using Baml2006Reader reader = new(stream);
            using XamlObjectWriter writer = new(reader.SchemaContext);

            while (reader.Read())
            {
                writer.WriteNode(reader);
            }
            return writer.Result;
        }
    }

    public static string Tr(this string key, params object[] args)
    {
        return string.Format(Tr(key)?.ToString(), args);
    }
}

public class TrFont
{
    public string? Name { get; set; }
    public string? TwoName { get; set; }
    public string? ThreeName { get; set; }

    public string? ResourceFontFileName { get; set; }
    public string? ResourceFontFamilyName { get; set; }
    public string? ResourceFamilyName => !string.IsNullOrWhiteSpace(ResourceFontFileName) && !string.IsNullOrWhiteSpace(ResourceFontFamilyName) ? $"./{ResourceFontFileName}#{ResourceFontFamilyName}" : null!;

    public string? SystemFamilyName { get; set; }
    public string? SystemFamilyNameBackup { get; set; }
}

public static class TrFontExtension
{
    public static TrFont OnNameOf(this TrFont self, string name)
    {
        self.Name = name ?? throw new ArgumentNullException(nameof(name));
        self.TwoName = null!;
        self.ThreeName = null!;
        return self;
    }

    public static TrFont OnTwoNameOf(this TrFont self, string twoName)
    {
        self.Name = null!;
        self.TwoName = twoName ?? throw new ArgumentNullException(nameof(twoName));
        self.ThreeName = null!;
        return self;
    }

    public static TrFont OnThreeNameOf(this TrFont self, string threeName)
    {
        self.Name = null!;
        self.TwoName = null!;
        self.ThreeName = threeName ?? throw new ArgumentNullException(nameof(threeName));
        return self;
    }

    public static TrFont OnAnyName(this TrFont self)
    {
        self.Name = null!;
        self.TwoName = null!;
        self.ThreeName = null!;
        return self;
    }

    public static TrFont ForResourceFont(this TrFont self, string fontFileName, string familyName)
    {
        self.ResourceFontFileName = fontFileName ?? throw new ArgumentNullException(nameof(fontFileName));
        self.ResourceFontFamilyName = familyName ?? throw new ArgumentNullException(nameof(familyName));
        return self;
    }

    public static TrFont ForSystemFont(this TrFont self, string familyName, string familyNameBackup = null!)
    {
        self.SystemFamilyName = familyName ?? throw new ArgumentNullException(nameof(familyName));
        self.SystemFamilyNameBackup = familyNameBackup;
        _ = !new Regex("^[a-zA-Z ]+$").IsMatch(familyName) ? throw new ArgumentException(nameof(familyName)) : default(object);
        return self;
    }
}
