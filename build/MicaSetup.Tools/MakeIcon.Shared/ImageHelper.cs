using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FontFamily = System.Drawing.FontFamily;
using FontStyleX = System.Drawing.FontStyle;

namespace MakeIcon.Shared;

public static class ImageHelper
{
    public static Bitmap OpenImage(IconType type, FontFamily fontFamily, string filename = "Favicon.png", string? changedColor = null)
    {
        Bitmap bitmap = new(256, 256);

        bitmap.AddImage(new Bitmap(filename), 0, 0, 256, 256);

        bitmap = bitmap.ChangeColor(changedColor?.ToColor());

        if (type == IconType.Setup)
        {
            // Circle
            bitmap.AddIconFont(Selection.Circle, 100, fontFamily, FontStyleX.Regular, "#EE24CDB9".ToColor(), 70, 75);

            // Up
            bitmap.AddIconFont(Selection.GallerySortReverse, 75, fontFamily, FontStyleX.Bold, "#FFFFFF".ToColor(), 70, 75);
        }
        else if (type == IconType.Uninst)
        {
            // Circle
            bitmap.AddIconFont(Selection.Circle, 100, fontFamily, FontStyleX.Regular, "#EEEB3B3B".ToColor(), 70, 75);

            // Close
            bitmap.AddIconFont(Selection.PublicCancelFilled, 75, fontFamily, FontStyleX.Bold, "#FFFFFF".ToColor(), 70, 75);
        }
        return bitmap;
    }

    public static void SaveImage(IconType type, FontFamily fontFamily, string filename = "Favicon.png", string ext = ".png", int[]? size = null, string? changedColor = null)
    {
        using Bitmap bitmap = OpenImage(type, fontFamily, filename, changedColor);
        string pathNoExt = $"Favicon{type switch
        {
            IconType.Setup => nameof(IconType.Setup),
            IconType.Uninst => nameof(IconType.Uninst),
            _ => string.Empty,
        }}";
        string output = Path.Combine(Path.GetDirectoryName(filename), $"{pathNoExt}{ext}");

        try
        {
            if (output.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                if (Path.GetFullPath(filename) == Path.GetFullPath(output))
                {
                    // Skip if the file is the same
                    return;
                }

                if (File.Exists(output))
                {
                    File.Delete(output);
                }
                bitmap.Save(output, ImageFormat.Png);
            }
        }
        catch
        {
            ///
        }

        try
        {
            if (output.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(output))
                {
                    File.Delete(output);
                }
                bitmap.ConvertToIco(output, size ?? [256]);
            }
        }
        catch
        {
            ///
        }
    }
}
