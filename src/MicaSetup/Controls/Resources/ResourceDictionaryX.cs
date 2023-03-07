using MicaSetup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MicaSetup.Controls;

[Obsolete]
public class ResourceDictionaryX : ResourceDictionary
{
    public new Uri Source
    {
        get => base.Source;
        set
        {
            if (Pack.Current.Uninst)
            {
                base.Source = new Uri(value.OriginalString.Replace("MicaSetup;component", "Uninst;component"));
                return;
            }
            base.Source = value;
        }
    }
}
