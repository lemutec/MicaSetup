using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Squircle.Core;

internal static class ImageWindowsFormsExtension
{
    public static void AddIconFont(this Bitmap bitmap, string text, float fontSize, FontFamily fontFamily, int offsetX = 0, int offsetY = 0)
    {
        using Graphics g = Graphics.FromImage(bitmap);
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        using Font font = new(fontFamily, fontSize, FontStyle.Regular);

        using Brush brush = new SolidBrush(Color.Black);
        SizeF textSize = g.MeasureString(text, font);
        float centerX = (bitmap.Width - textSize.Width) / 2f;
        float centerY = (bitmap.Height - textSize.Height) / 2f;

        g.DrawString(text, font, brush, new PointF(centerX + offsetX, centerY + offsetY));
    }
}
