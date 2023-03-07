using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Squircle.Core;

internal static class SquircleExtension
{
    public static Bitmap Create(int w = 1024, int h = 1024, Color? color = null, int margin = 0, double n = 3d, double m = 3d)
    {
        using Bitmap bitmap = new(w, h);
        using Graphics g = Graphics.FromImage(bitmap);
        using Bitmap? bitmap1 = CreateQuarter(w / 4, h / 4, color, margin, n, m);
        using Bitmap? bitmap2 = bitmap1.Clone() as Bitmap;
        using Bitmap? bitmap3 = bitmap1.Clone() as Bitmap;
        using Bitmap? bitmap4 = bitmap1.Clone() as Bitmap;

        bitmap1!.RotateFlip(RotateFlipType.Rotate180FlipNone);
        bitmap2!.RotateFlip(RotateFlipType.Rotate270FlipNone);
        bitmap3!.RotateFlip(RotateFlipType.RotateNoneFlipNone);
        bitmap4!.RotateFlip(RotateFlipType.Rotate90FlipNone);

        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
        g.DrawImageUnscaled(bitmap1, margin, margin, w / 4, w / 4);
        g.DrawImageUnscaled(bitmap2, margin + w / 4, margin, w / 4, w / 4);
        g.DrawImageUnscaled(bitmap3, margin + w / 4, margin + w / 4, w / 4, w / 4);
        g.DrawImageUnscaled(bitmap4, margin, margin + w / 4, w / 4, w / 4);

        int quarterWidth = bitmap.Width / 2;
        int quarterHeight = bitmap.Height / 2;
        Rectangle rect = new(0, 0, quarterWidth, quarterHeight);
        return bitmap.Clone(rect, bitmap.PixelFormat);
    }

    public static Bitmap CreateQuarter(int w = 256, int h = 256, Color? color = null, int margin = 0, double n = 3d, double m = 3d)
    {
        Bitmap bitmap = new(w, h);
        using Graphics g = Graphics.FromImage(bitmap);
        using Brush brush = new SolidBrush(color ?? Color.White);
        using GraphicsPath gp = new();

        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
        gp.AddClosedCurve(Calc2d(w - margin, h - margin, n, m));
        g.FillPath(brush, gp);
        return bitmap;
    }

    internal static PointF[] Calc2dLegacy(double w, double h, double n = 3d)
    {
        static double sgn(double n) => n > 0d ? 1d : n < 0d ? -1d : 0d;
        static double abs(double v) => Math.Abs(v);
        static double cos(double v) => Math.Cos(v);
        static double sin(double v) => Math.Sin(v);
        static double pow(double x, double y) => Math.Pow(x, y);
        double na = 2d / n;
        double step = 3000d;
        double piece = Math.PI * 2d / step;
        List<PointF> p = new();

        double t = 0;
        for (int i = default; i < step + 1; i++)
        {
            double x = pow(abs(cos(t)), na) * w * sgn(cos(t));
            double y = pow(abs(sin(t)), na) * h * sgn(sin(t));

            p.Add(new((float)x, (float)y));
            t += piece;
        }
        return p.ToArray();
    }

    internal static PointF[] Calc2d(double w, double h, double n = 3d, double m = 3d)
    {
        static double sgn(double n) => n > 0d ? 1d : n < 0d ? -1d : 0d;
        static double abs(double v) => Math.Abs(v);
        static double cos(double v) => Math.Cos(v);
        static double sin(double v) => Math.Sin(v);
        static double pow(double x, double y) => Math.Pow(x, y);
        double na = 2d / n;
        double ma = 2d / m;
        double step = 3000d;
        double piece = Math.PI * 2d / step;
        List<PointF> p = new();

        double t = 0;
        for (int i = default; i < step + 1; i++)
        {
            double x = pow(abs(cos(t)), na) * pow(abs(cos(t)), ma) * w * sgn(cos(t));
            double y = pow(abs(sin(t)), na) * pow(abs(sin(t)), ma) * h * sgn(sin(t));

            p.Add(new((float)x, (float)y));
            t += piece;
        }
        return p.ToArray();
    }
}
