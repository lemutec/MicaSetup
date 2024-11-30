using MakeMica.Shared;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace MakeMica.Cli.Core;

public static class CSharpResource
{
    public const string SubFolderOfFonts = "Fonts";
    public const string SubFolderOfImages = "Images";

    public static void SetupConfig(string resourceDir, MicaConfig config)
    {
        if (File.Exists(config.Package))
        {
            string package = Path.Combine(resourceDir, "Setups", "publish.7z");
            File.Copy(config.Package, package, true);
        }

        if (!string.IsNullOrWhiteSpace(config.LicenseFile) || !string.IsNullOrWhiteSpace(config.License))
        {
            foreach (string file in Directory.GetFiles(Path.Combine(resourceDir, "Licenses"), "*.txt"))
            {
                File.Delete(file);
            }

            string licenseFile = MicaMacro.GetFullPath(config.LicenseFile);
            string license = Path.Combine(resourceDir, "Licenses", "license.txt");

            if (File.Exists(licenseFile))
            {
                File.Copy(licenseFile, license, true);
            }
            else
            {
                File.WriteAllText(license, config.License);
            }
        }

        if (!string.IsNullOrWhiteSpace(config.Favicon))
        {
            string favicon = MicaMacro.GetFullPath(config.Favicon);

            if (File.Exists(favicon))
            {
                string imageDir = Path.Combine(resourceDir, "Images");

                if (Path.GetExtension(favicon).ToLower() == ".png")
                {
                    File.Delete(Path.Combine(imageDir, "Favicon.png"));
                    File.Delete(Path.Combine(imageDir, "Favicon.ico"));
                    File.Copy(favicon, Path.Combine(imageDir, "Favicon.png"), true);
                    ImageExtension.OpenImage(favicon).ConvertToIco(Path.Combine(imageDir, "Favicon.ico"));
                }
                else if (Path.GetExtension(favicon).ToLower() == ".ico")
                {
                    File.Delete(Path.Combine(imageDir, "Favicon.png"));
                    File.Delete(Path.Combine(imageDir, "Favicon.ico"));
                    File.Copy(favicon, Path.Combine(imageDir, "Favicon.ico"), true);
                    ImageExtension.OpenIcon(favicon).ConvertToPng(Path.Combine(imageDir, "Favicon.png"));
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(config.Icon))
        {
            string icon = MicaMacro.GetFullPath(config.Icon);

            if (File.Exists(icon))
            {
                string imageDir = Path.Combine(resourceDir, "Images");

                if (Path.GetExtension(icon).ToLower() == ".png")
                {
                    File.Delete(Path.Combine(imageDir, "FaviconSetup.png"));
                    File.Delete(Path.Combine(imageDir, "FaviconSetup.ico"));
                    File.Copy(icon, Path.Combine(imageDir, "FaviconSetup.png"), true);
                    ImageExtension.OpenImage(icon).ConvertToIco(Path.Combine(imageDir, "FaviconSetup.ico"));
                }
                else if (Path.GetExtension(icon).ToLower() == ".ico")
                {
                    File.Delete(Path.Combine(imageDir, "FaviconSetup.png"));
                    File.Delete(Path.Combine(imageDir, "FaviconSetup.ico"));
                    File.Copy(icon, Path.Combine(imageDir, "FaviconSetup.ico"), true);
                    ImageExtension.OpenIcon(icon).ConvertToPng(Path.Combine(imageDir, "FaviconSetup.png"));
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(config.UnIcon))
        {
            string unIcon = MicaMacro.GetFullPath(config.UnIcon);

            if (File.Exists(unIcon))
            {
                string imageDir = Path.Combine(resourceDir, "Images");

                if (Path.GetExtension(unIcon).ToLower() == ".png")
                {
                    File.Delete(Path.Combine(imageDir, "FaviconUninst.png"));
                    File.Delete(Path.Combine(imageDir, "FaviconUninst.ico"));
                    File.Copy(unIcon, Path.Combine(imageDir, "FaviconUninst.png"), true);
                    ImageExtension.OpenImage(unIcon).ConvertToIco(Path.Combine(imageDir, "FaviconUninst.ico"));
                }
                else if (Path.GetExtension(unIcon).ToLower() == ".ico")
                {
                    File.Delete(Path.Combine(imageDir, "FaviconUninst.png"));
                    File.Delete(Path.Combine(imageDir, "FaviconUninst.ico"));
                    File.Copy(unIcon, Path.Combine(imageDir, "FaviconUninst.ico"), true);
                    ImageExtension.OpenIcon(unIcon).ConvertToPng(Path.Combine(imageDir, "FaviconUninst.png"));
                }
            }
        }
    }
}

file static class ImageExtension
{
    public static Bitmap OpenImage(string filename)
    {
        Bitmap bitmap = new(256, 256);

        bitmap.AddImage(new Bitmap(filename), 0, 0, 256, 256);
        return bitmap;
    }

    public static void AddImage(this Bitmap bitmap, Bitmap overlay, int offsetX = 0, int offsetY = 0, int width = 0, int height = 0)
    {
        using Graphics g = Graphics.FromImage(bitmap);
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.DrawImage(overlay, offsetX, offsetY, width, height);
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

    public static Icon OpenIcon(string filename)
    {
        Icon icon = new(filename);
        return icon;
    }

    public static void ConvertToPng(this Icon icon, string filePath)
    {
        using Bitmap bitmap = icon.ToBitmap();
        using MemoryStream memoryStream = new();
        bitmap.Save(memoryStream, ImageFormat.Png);

        using FileStream fileStream = new(filePath, FileMode.Create);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(fileStream);
    }
}
