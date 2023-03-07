using MicaSetup.Core;
using System;
using System.Windows;

namespace MicaSetup.Controls;

public sealed class ConverterResourceDictionary : ResourceDictionary
{
    public ConverterResourceDictionary()
    {
        Source = new Uri($"pack://application:,,,/MicaSetup;component/Resources/Converter.xaml");
    }
}
