using System;
using System.Windows;

namespace MicaSetup.Controls;

public sealed class ResourceResourceDictionary : ResourceDictionary
{
    public ResourceResourceDictionary()
    {
        Source = new Uri($"pack://application:,,,/MicaSetup;component/Resources/Resource.xaml");
    }
}
