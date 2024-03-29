﻿using System.Windows;

namespace MicaSetup.Controls;

public static class WindowExtension
{
    public static void EnableBackdrop(this Window window, BackdropType backdropType = BackdropType.Mica)
    {
        ThemeService.Current.EnableBackdrop(window, backdropType);
    }
}
