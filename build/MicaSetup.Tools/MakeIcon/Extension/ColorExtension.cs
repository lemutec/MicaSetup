using System.Drawing;
using Color = System.Drawing.Color;

namespace MakeIcon.Extension;

internal static class ColorExtension
{
    public static Color ToColor(this string htmlColor)
    {
        return ColorTranslator.FromHtml(htmlColor);
    }
}
