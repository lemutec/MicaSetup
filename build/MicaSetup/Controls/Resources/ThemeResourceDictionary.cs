using System;
using System.Windows;

namespace MicaSetup.Controls;

public sealed class ThemeResourceDictionary : ResourceDictionary
{
    public ThemeResourceDictionary()
    {
        MergedDictionaries.Add(new ResourceDictionary()
        {
            Source = new Uri($"pack://application:,,,/MicaSetup;component/Resources/Themes/Dark.xaml"),
        });
        MergedDictionaries.Add(new ResourceDictionary()
        {
            Source = new Uri($"pack://application:,,,/MicaSetup;component/Resources/Themes/Light.xaml"),
        });
        MergedDictionaries.Add(new ResourceDictionary()
        {
            Source = new Uri($"pack://application:,,,/MicaSetup;component/Resources/Themes/Brushes.xaml"),
        });
    }
}
