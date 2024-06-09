using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace MicaSquircle.Extension;

internal static class ImageGdiExtension
{
    public static void AddImage(this Bitmap bitmap, Bitmap overlay, int offsetX = 0, int offsetY = 0, int width = 0, int height = 0)
    {
        using Graphics g = Graphics.FromImage(bitmap);
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.DrawImage(overlay, offsetX, offsetY, width, height);
    }

    public static void AddIconFont(this Bitmap bitmap, string text, float fontSize, FontFamily fontFamily, FontStyle fontStyle = FontStyle.Regular, Color? color = null!, int offsetX = 0, int offsetY = 0)
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
        using Font font = new(fontFamily, fontSize, fontStyle);
        using Brush brush = new SolidBrush(color ?? Color.Black);

        RectangleF textBounds = new(PointF.Empty, g.MeasureString(text, font));
        PointF center = new(bitmap.Width / 2f, bitmap.Height / 2f);

        center.X -= textBounds.Width / 2f;
        center.Y -= textBounds.Height / 2f;
        g.DrawString(text, font, brush, center.X + offsetX, center.Y + offsetY);
    }

    public static Bitmap DrawFrame(this Bitmap bitmap, Color? color = null!, int thickness = 1)
    {
        using Graphics g = Graphics.FromImage(bitmap);
        using Pen pen = new(color ?? Color.Black, thickness);
        g.DrawRectangle(pen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
        return bitmap;
    }

    /// <summary>
    /// https://ja.jinzhao.wiki/wiki/ICO_(%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%83%95%E3%82%A9%E3%83%BC%E3%83%9E%E3%83%83%E3%83%88)
    /// </summary>
    public static void ConvertToIco(this Image bitmap, string filePath, int[]? sizes = null)
    {
        sizes ??= [256, 64, 48, 32, 24, 16];

        using MemoryStream memoryStream = new();
        using BinaryWriter binaryWriter = new(memoryStream);
        Bitmap[] pngs = new Bitmap[sizes.Length];
        int offset = 6 + sizes.Length * 16;

        binaryWriter.Write((short)0);
        binaryWriter.Write((short)1);
        binaryWriter.Write((short)sizes.Length);

        for (int i = default; i < sizes.Length; i++)
        {
            using MemoryStream memoryStream1 = new();
            Bitmap bitmap1 = new(bitmap, new Size(sizes[i], sizes[i]));
            bitmap1.Save(memoryStream1, ImageFormat.Png);
            pngs[i] = bitmap1;

            binaryWriter.Write((byte)sizes[i]);
            binaryWriter.Write((byte)sizes[i]);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((short)1);
            binaryWriter.Write((short)32);
            binaryWriter.Write((int)memoryStream1.Length);
            binaryWriter.Write(offset);

            offset += (int)memoryStream1.Length;
        }

        for (int i = default; i < pngs.Length; i++)
        {
            using Bitmap bitmap1 = pngs[i];
            bitmap1.Save(memoryStream, ImageFormat.Png);
        }

        using FileStream fileStream = new(filePath, FileMode.Create);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(fileStream);
    }
}
