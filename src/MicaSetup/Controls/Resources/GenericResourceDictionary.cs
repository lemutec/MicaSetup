using MicaSetup.Core;
using System;
using System.Windows;

namespace MicaSetup.Controls;

public sealed class GenericResourceDictionary : ResourceDictionary
{
    public GenericResourceDictionary()
    {
        Source = new Uri($"pack://application:,,,/MicaSetup;component/Resources/Generic.xaml");
    }
}
