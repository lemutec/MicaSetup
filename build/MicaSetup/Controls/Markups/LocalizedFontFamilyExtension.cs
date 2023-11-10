using MicaSetup.Services;
using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace MicaSetup.Controls;

[MarkupExtensionReturnType(typeof(FontFamily))]
public class LocalizedFontFamilyExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return ServiceManager.GetService<IMuiLanguageService>()?.GetFontFamily() ?? new FontFamily();
    }
}
