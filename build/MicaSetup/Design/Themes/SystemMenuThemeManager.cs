using MicaSetup.Helper;
using MicaSetup.Natives;
using Microsoft.Win32;
using System;

namespace MicaSetup.Design.Themes;

internal static class SystemMenuThemeManager
{
    public static void Apply(SystemMenuTheme theme = SystemMenuTheme.Auto)
    {
        // Enable dark mode for context menus if using dark theme
        if (Environment.OSVersion.Version.Build >= 18362) // Windows 10 1903
        {
            if (theme == SystemMenuTheme.Auto)
            {
                // UxTheme methods will apply all of menus.
                // However, the Windows style system prefers that
                // Windows System Menu is based on `Apps Theme`,
                // and Tray Context Menu is based on `System Theme` when using a custom theme.
                // But actually we can't have our cake and eat it too.
                // Finally, we synchronize the theme styles of tray with higher usage rates.
                if (OSThemeHelper.SystemUsesDarkTheme())
                {
                    _ = UxTheme.SetPreferredAppMode(UxTheme.PreferredAppMode.ForceDark);
                    UxTheme.FlushMenuThemes();
                }

                // Synchronize the theme with system settings
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChangedEventHandler;
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChangedEventHandler;
            }
            else if (theme == SystemMenuTheme.Dark)
            {
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChangedEventHandler;
                _ = UxTheme.SetPreferredAppMode(UxTheme.PreferredAppMode.ForceDark);
                UxTheme.FlushMenuThemes();
            }
            else if (theme == SystemMenuTheme.Light)
            {
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChangedEventHandler;
                _ = UxTheme.SetPreferredAppMode(UxTheme.PreferredAppMode.ForceLight);
                UxTheme.FlushMenuThemes();
            }
        }
    }

    private static void OnUserPreferenceChangedEventHandler(object sender, UserPreferenceChangedEventArgs e)
    {
        if (OSThemeHelper.SystemUsesDarkTheme())
        {
            _ = UxTheme.SetPreferredAppMode(UxTheme.PreferredAppMode.ForceDark);
            UxTheme.FlushMenuThemes();
        }
        else
        {
            _ = UxTheme.SetPreferredAppMode(UxTheme.PreferredAppMode.ForceLight);
            UxTheme.FlushMenuThemes();
        }
    }
}

/// <summary>
/// Theme in which an system menu is displayed.
/// </summary>
public enum SystemMenuTheme
{
    /// <summary>
    /// Auto system theme.
    /// </summary>
    Auto = ApplicationTheme.Unknown,

    /// <summary>
    /// Dark system theme.
    /// </summary>
    Dark = ApplicationTheme.Dark,

    /// <summary>
    /// Light system theme.
    /// </summary>
    Light = ApplicationTheme.Light,
}
