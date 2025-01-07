using Microsoft.Win32;

namespace MicaSetup.Helper;

public static class OSThemeHelper
{
    public static bool AppsUseDarkTheme()
    {
        object? value = Registry.GetValue(
                            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                            "AppsUseLightTheme", 1);

        return value != null && (int)value == 0;
    }

    public static bool SystemUsesDarkTheme()
    {
        object? value = Registry.GetValue(
                            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                            "SystemUsesLightTheme", 0);

        return value == null || (int)value == 0;
    }
}
