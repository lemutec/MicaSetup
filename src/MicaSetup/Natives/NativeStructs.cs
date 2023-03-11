using System;
using System.Runtime.InteropServices;

namespace MicaSetup.Win32;

[StructLayout(LayoutKind.Sequential), Serializable]
[ComVisible(true)]
public struct POINT : IEquatable<POINT>
{
    public int X;
    public int Y;

    public POINT(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public int x { get => x; set => x = value; }
    public int y { get => y; set => y = value; }

    public void Offset(int dx, int dy)
    {
        X += dx;
        Y += dy;
    }

    public bool Equals(POINT other) => other.X == X && other.Y == Y;
    public override int GetHashCode() => unchecked(X ^ Y);
    public override string ToString() => $"{{X={X},Y={Y}}}";
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT : IEquatable<RECT>
{
    public int left;
    public int top;
    public int right;
    public int bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    public int Left { get => left; set => left = value; }
    public int Right { get => right; set => right = value; }
    public int Top { get => top; set => top = value; }
    public int Bottom { get => bottom; set => bottom = value; }

    public int X
    {
        get => left;
        set
        {
            right -= left - value;
            left = value;
        }
    }
    
    public int Y
    {
        get => top;
        set
        {
            bottom -= top - value;
            top = value;
        }
    }

    public int Height
    {
        get => bottom - top;
        set => bottom = value + top;
    }

    public int Width
    {
        get => right - left;
        set => right = value + left;
    }

    public POINT Location
    {
        get => new(left, top);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public SIZE Size
    {
        get => new(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public bool IsEmpty => left == 0 && top == 0 && right == 0 && bottom == 0;

    public bool Equals(RECT r) => r.left == left && r.top == top && r.right == right && r.bottom == bottom;

    public override string ToString() => $"{{left={left},top={top},right={right},bottom={bottom}}}";
    public static readonly RECT Empty = new();
}

[StructLayout(LayoutKind.Sequential), Serializable]
public struct SIZE : IEquatable<SIZE>
{
    public int cx;
    public int cy;
    
    public SIZE(int width, int height)
    {
        cx = width;
        cy = height;
    }

    public int Height { get => cy; set => cy = value; }
    public bool IsEmpty => cx == 0 && cy == 0;
    public int Width { get => cx; set => cx = value; }

    public bool Equals(SIZE other) => cx == other.cx || cy == other.cy;
    public override int GetHashCode() => IsEmpty ? 0 : cx.GetHashCode() ^ cy.GetHashCode();
    public override string ToString() => $"{{cx={cx}, cy={cy}}}";

    public static readonly SIZE Empty = new();
}

[StructLayout(LayoutKind.Sequential)]
public struct OSVERSIONINFOEX
{
    public int OSVersionInfoSize;
    public int MajorVersion;
    public int MinorVersion;
    public int BuildNumber;
    public int Revision;
    public int PlatformId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string CSDVersion;
    public ushort ServicePackMajor;
    public ushort ServicePackMinor;
    public short SuiteMask;
    public byte ProductType;
    public byte Reserved;
}
