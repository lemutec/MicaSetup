using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vanara.PInvoke;

namespace MakeIcon.Extension;

internal static class ImageExtension
{
    public static ImageSource ToImageSource(this Bitmap bitmap)
    {
        nint hBitmap = bitmap.GetHbitmap();
        ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        _ = !Gdi32.DeleteObject(hBitmap);
        return wpfBitmap;
    }
}
