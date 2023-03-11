﻿using System;

namespace MicaSetup.Win32;

public enum WindowMessage
{
    WM_NULL = 0x0000,
    WM_CREATE = 0x0001,
    WM_DESTROY = 0x0002,
    WM_MOVE = 0x0003,
    WM_SIZE = 0x0005,
    WM_ACTIVATE = 0x0006,
    WM_SETFOCUS = 0x0007,
    WM_KILLFOCUS = 0x0008,
    WM_ENABLE = 0x000A,
    WM_SETREDRAW = 0x000B,
    WM_SETTEXT = 0x000C,
    WM_GETTEXT = 0x000D,
    WM_GETTEXTLENGTH = 0x000E,
    WM_PAINT = 0x000F,
    WM_CLOSE = 0x0010,
    WM_QUERYENDSESSION = 0x0011,
    WM_QUIT = 0x0012,
    WM_QUERYOPEN = 0x0013,
    WM_ERASEBKGND = 0x0014,
    WM_SYSCOLORCHANGE = 0x0015,
    WM_ENDSESSION = 0x0016,
    WM_SHOWWINDOW = 0x0018,
    WM_CTLCOLOR = 0x0019,
    WM_WININICHANGE = 0x001A,
    WM_SETTINGCHANGE = 0x001A,
    WM_DEVMODECHANGE = 0x001B,
    WM_ACTIVATEAPP = 0x001C,
    WM_FONTCHANGE = 0x001D,
    WM_TIMECHANGE = 0x001E,
    WM_CANCELMODE = 0x001F,
    WM_SETCURSOR = 0x0020,
    WM_MOUSEACTIVATE = 0x0021,
    WM_CHILDACTIVATE = 0x0022,
    WM_QUEUESYNC = 0x0023,
    WM_GETMINMAXINFO = 0x0024,
    WM_PAINTICON = 0x0026,
    WM_ICONERASEBKGND = 0x0027,
    WM_NEXTDLGCTL = 0x0028,
    WM_SPOOLERSTATUS = 0x002A,
    WM_DRAWITEM = 0x002B,
    WM_MEASUREITEM = 0x002C,
    WM_DELETEITEM = 0x002D,
    WM_VKEYTOITEM = 0x002E,
    WM_CHARTOITEM = 0x002F,
    WM_SETFONT = 0x0030,
    WM_GETFONT = 0x0031,
    WM_SETHOTKEY = 0x0032,
    WM_GETHOTKEY = 0x0033,
    WM_QUERYDRAGICON = 0x0037,
    WM_COMPAREITEM = 0x0039,
    WM_GETOBJECT = 0x003D,
    WM_COMPACTING = 0x0041,
    WM_COMMNOTIFY = 0x0044,
    WM_WINDOWPOSCHANGING = 0x0046,
    WM_WINDOWPOSCHANGED = 0x0047,
    WM_POWER = 0x0048,
    WM_COPYDATA = 0x004A,
    WM_CANCELJOURNAL = 0x004B,
    WM_NOTIFY = 0x004E,
    WM_INPUTLANGCHANGEREQUEST = 0x0050,
    WM_INPUTLANGCHANGE = 0x0051,
    WM_TCARD = 0x0052,
    WM_HELP = 0x0053,
    WM_USERCHANGED = 0x0054,
    WM_NOTIFYFORMAT = 0x0055,
    WM_CONTEXTMENU = 0x007B,
    WM_STYLECHANGING = 0x007C,
    WM_STYLECHANGED = 0x007D,
    WM_DISPLAYCHANGE = 0x007E,
    WM_GETICON = 0x007F,
    WM_SETICON = 0x0080,
    WM_NCCREATE = 0x0081,
    WM_NCDESTROY = 0x0082,
    WM_NCCALCSIZE = 0x0083,
    WM_NCHITTEST = 0x0084,
    WM_NCPAINT = 0x0085,
    WM_NCACTIVATE = 0x0086,
    WM_GETDLGCODE = 0x0087,
    WM_SYNCPAINT = 0x0088,
    WM_NCMOUSEMOVE = 0x00A0,
    WM_NCLBUTTONDOWN = 0x00A1,
    WM_NCLBUTTONUP = 0x00A2,
    WM_NCLBUTTONDBLCLK = 0x00A3,
    WM_NCRBUTTONDOWN = 0x00A4,
    WM_NCRBUTTONUP = 0x00A5,
    WM_NCRBUTTONDBLCLK = 0x00A6,
    WM_NCMBUTTONDOWN = 0x00A7,
    WM_NCMBUTTONUP = 0x00A8,
    WM_NCMBUTTONDBLCLK = 0x00A9,
    WM_KEYDOWN = 0x0100,
    WM_KEYUP = 0x0101,
    WM_CHAR = 0x0102,
    WM_DEADCHAR = 0x0103,
    WM_SYSKEYDOWN = 0x0104,
    WM_SYSKEYUP = 0x0105,
    WM_SYSCHAR = 0x0106,
    WM_SYSDEADCHAR = 0x0107,
    WM_KEYLAST = 0x0108,
    WM_IME_STARTCOMPOSITION = 0x010D,
    WM_IME_ENDCOMPOSITION = 0x010E,
    WM_IME_COMPOSITION = 0x010F,
    WM_IME_KEYLAST = 0x010F,
    WM_INITDIALOG = 0x0110,
    WM_COMMAND = 0x0111,
    WM_SYSCOMMAND = 0x0112,
    WM_TIMER = 0x0113,
    WM_HSCROLL = 0x0114,
    WM_VSCROLL = 0x0115,
    WM_INITMENU = 0x0116,
    WM_INITMENUPOPUP = 0x0117,
    WM_MENUSELECT = 0x011F,
    WM_MENUCHAR = 0x0120,
    WM_ENTERIDLE = 0x0121,
    WM_MENURBUTTONUP = 0x0122,
    WM_MENUDRAG = 0x0123,
    WM_MENUGETOBJECT = 0x0124,
    WM_UNINITMENUPOPUP = 0x0125,
    WM_MENUCOMMAND = 0x0126,
    WM_CTLCOLORWinMsgBOX = 0x0132,
    WM_CTLCOLOREDIT = 0x0133,
    WM_CTLCOLORLISTBOX = 0x0134,
    WM_CTLCOLORBTN = 0x0135,
    WM_CTLCOLORDLG = 0x0136,
    WM_CTLCOLORSCROLLBAR = 0x0137,
    WM_CTLCOLORSTATIC = 0x0138,
    WM_MOUSEMOVE = 0x0200,
    WM_LBUTTONDOWN = 0x0201,
    WM_LBUTTONUP = 0x0202,
    WM_LBUTTONDBLCLK = 0x0203,
    WM_RBUTTONDOWN = 0x0204,
    WM_RBUTTONUP = 0x0205,
    WM_RBUTTONDBLCLK = 0x0206,
    WM_MBUTTONDOWN = 0x0207,
    WM_MBUTTONUP = 0x0208,
    WM_MBUTTONDBLCLK = 0x0209,
    WM_MOUSEWHEEL = 0x020A,
    WM_PARENTNOTIFY = 0x0210,
    WM_ENTERMENULOOP = 0x0211,
    WM_EXITMENULOOP = 0x0212,
    WM_NEXTMENU = 0x0213,
    WM_SIZING = 0x0214,
    WM_CAPTURECHANGED = 0x0215,
    WM_MOVING = 0x0216,
    WM_DEVICECHANGE = 0x0219,
    WM_MDICREATE = 0x0220,
    WM_MDIDESTROY = 0x0221,
    WM_MDIACTIVATE = 0x0222,
    WM_MDIRESTORE = 0x0223,
    WM_MDINEXT = 0x0224,
    WM_MDIMAXIMIZE = 0x0225,
    WM_MDITILE = 0x0226,
    WM_MDICASCADE = 0x0227,
    WM_MDIICONARRANGE = 0x0228,
    WM_MDIGETACTIVE = 0x0229,
    WM_MDISETMENU = 0x0230,
    WM_ENTERSIZEMOVE = 0x0231,
    WM_EXITSIZEMOVE = 0x0232,
    WM_DROPFILES = 0x0233,
    WM_MDIREFRESHMENU = 0x0234,
    WM_IME_SETCONTEXT = 0x0281,
    WM_IME_NOTIFY = 0x0282,
    WM_IME_CONTROL = 0x0283,
    WM_IME_COMPOSITIONFULL = 0x0284,
    WM_IME_SELECT = 0x0285,
    WM_IME_CHAR = 0x0286,
    WM_IME_REQUEST = 0x0288,
    WM_IME_KEYDOWN = 0x0290,
    WM_IME_KEYUP = 0x0291,
    WM_MOUSEHOVER = 0x02A1,
    WM_MOUSELEAVE = 0x02A3,
    WM_DPICHANGED = 0x02E0,
    WM_CUT = 0x0300,
    WM_COPY = 0x0301,
    WM_PASTE = 0x0302,
    WM_CLEAR = 0x0303,
    WM_UNDO = 0x0304,
    WM_RENDERFORMAT = 0x0305,
    WM_RENDERALLFORMATS = 0x0306,
    WM_DESTROYCLIPBOARD = 0x0307,
    WM_DRAWCLIPBOARD = 0x0308,
    WM_PAINTCLIPBOARD = 0x0309,
    WM_VSCROLLCLIPBOARD = 0x030A,
    WM_SIZECLIPBOARD = 0x030B,
    WM_ASKCBFORMATNAME = 0x030C,
    WM_CHANGECBCHAIN = 0x030D,
    WM_HSCROLLCLIPBOARD = 0x030E,
    WM_QUERYNEWPALETTE = 0x030F,
    WM_PALETTEISCHANGING = 0x0310,
    WM_PALETTECHANGED = 0x0311,
    WM_HOTKEY = 0x0312,
    WM_PRINT = 0x0317,
    WM_PRINTCLIENT = 0x0318,
    WM_HANDHELDFIRST = 0x0358,
    WM_HANDHELDLAST = 0x035F,
    WM_AFXFIRST = 0x0360,
    WM_AFXLAST = 0x037F,
    WM_PENWINFIRST = 0x0380,
    WM_PENWINLAST = 0x038F,
    WM_APP = 0x8000,
    WM_USER = 0x0400,
    WM_REFLECT = WM_USER + 0x1c00,
}

public enum HitTestValues
{
    HTERROR = -2,
    HTTRANSPARENT = -1,
    HTNOWHERE = 0,
    HTCLIENT = 1,
    HTCAPTION = 2,
    HTSYSMENU = 3,
    HTGROWBOX = 4,
    HTSIZE = HTGROWBOX,
    HTMENU = 5,
    HTHSCROLL = 6,
    HTVSCROLL = 7,
    HTMINBUTTON = 8,
    HTMAXBUTTON = 9,
    HTLEFT = 10,
    HTRIGHT = 11,
    HTTOP = 12,
    HTTOPLEFT = 13,
    HTTOPRIGHT = 14,
    HTBOTTOM = 15,
    HTBOTTOMLEFT = 16,
    HTBOTTOMRIGHT = 17,
    HTBORDER = 18,
    HTREDUCE = HTMINBUTTON,
    HTZOOM = HTMAXBUTTON,
    HTSIZEFIRST = HTLEFT,
    HTSIZELAST = HTBOTTOMRIGHT,
    HTOBJECT = 19,
    HTCLOSE = 20,
    HTHELP = 21
}

public enum WindowLongFlags
{
    GWL_EXSTYLE = -20,
    GWL_HINSTANCE = -6,
    GWLP_HINSTANCE = -6,
    GWL_HWNDPARENT = -8,
    GWL_ID = -12,
    GWLP_ID = -12,
    GWL_STYLE = -16,
    GWL_USERDATA = -21,
    GWLP_USERDATA = -21,
    GWL_WNDPROC = -4,
    GWLP_WNDPROC = -4,
    DWLP_USER = 8,
    DWLP_MSGRESULT = 0,
    DWLP_DLGPROC = 4,
    DWL_USER = 8,
    DWL_MSGRESULT = 0,
    DWL_DLGPROC = 4
}

[Flags]
public enum WindowStyles : uint
{
    WS_BORDER = 0x800000u,
    WS_CAPTION = 0xC00000u,
    WS_CHILD = 0x40000000u,
    WS_CLIPCHILDREN = 0x2000000u,
    WS_CLIPSIBLINGS = 0x4000000u,
    WS_DISABLED = 0x8000000u,
    WS_DLGFRAME = 0x400000u,
    WS_GROUP = 0x20000u,
    WS_HSCROLL = 0x100000u,
    WS_MAXIMIZE = 0x1000000u,
    WS_MAXIMIZEBOX = 0x10000u,
    WS_MINIMIZE = 0x20000000u,
    WS_MINIMIZEBOX = 0x20000u,
    WS_OVERLAPPED = 0x0u,
    WS_OVERLAPPEDWINDOW = 0xCF0000u,
    WS_POPUP = 0x80000000u,
    WS_POPUPWINDOW = 0x80880000u,
    WS_THICKFRAME = 0x40000u,
    WS_SYSMENU = 0x80000u,
    WS_TABSTOP = 0x10000u,
    WS_VISIBLE = 0x10000000u,
    WS_VSCROLL = 0x200000u,
    WS_TILED = 0x0u,
    WS_ICONIC = 0x20000000u,
    WS_SIZEBOX = 0x40000u,
    WS_TILEDWINDOW = 0xCF0000u,
    WS_CHILDWINDOW = 0x40000000u
}

public enum DWMWINDOWATTRIBUTE
{
    DWMWA_NCRENDERING_ENABLED = 1,
    DWMWA_NCRENDERING_POLICY,
    DWMWA_TRANSITIONS_FORCEDISABLED,
    DWMWA_ALLOW_NCPAINT,
    DWMWA_CAPTION_BUTTON_BOUNDS,
    DWMWA_NONCLIENT_RTL_LAYOUT,
    DWMWA_FORCE_ICONIC_REPRESENTATION,
    DWMWA_FLIP3D_POLICY,
    DWMWA_EXTENDED_FRAME_BOUNDS,
    DWMWA_HAS_ICONIC_BITMAP,
    DWMWA_DISALLOW_PEEK,
    DWMWA_EXCLUDED_FROM_PEEK,
    DWMWA_CLOAK,
    DWMWA_CLOAKED,
    DWMWA_FREEZE_REPRESENTATION,
    DWMWA_USE_HOSTBACKDROPBRUSH,
    DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
    DWMWA_WINDOW_CORNER_PREFERENCE = 33,
    DWMWA_BORDER_COLOR,
    DWMWA_CAPTION_COLOR,
    DWMWA_TEXT_COLOR,
    DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
    DWMWA_SYSTEMBACKDROP_TYPE,
    DWMWA_MICA_EFFECT = 1029,
}

public enum PROCESS_DPI_AWARENESS
{
    PROCESS_DPI_UNAWARE,
    PROCESS_SYSTEM_DPI_AWARE,
    PROCESS_PER_MONITOR_DPI_AWARE,
}

public enum MonitorFlags
{
    MONITOR_DEFAULTTONULL,
    MONITOR_DEFAULTTOPRIMARY,
    MONITOR_DEFAULTTONEAREST,
}

public enum MONITOR_DPI_TYPE
{
    MDT_EFFECTIVE_DPI = 0,
    MDT_ANGULAR_DPI,
    MDT_RAW_DPI,
    MDT_DEFAULT = MDT_EFFECTIVE_DPI,
}

[Flags]
public enum SetWindowPosFlags : uint
{
    SWP_ASYNCWINDOWPOS = 0x4000,
    SWP_DEFERERASE = 0x2000,
    SWP_DRAWFRAME = 0x0020,
    SWP_FRAMECHANGED = 0x0020,
    SWP_HIDEWINDOW = 0x0080,
    SWP_NOACTIVATE = 0x0010,
    SWP_NOCOPYBITS = 0x0100,
    SWP_NOMOVE = 0x0002,
    SWP_NOOWNERZORDER = 0x0200,
    SWP_NOREDRAW = 0x0008,
    SWP_NOREPOSITION = 0x0200,
    SWP_NOSENDCHANGING = 0x0400,
    SWP_NOSIZE = 0x0001,
    SWP_NOZORDER = 0x0004,
    SWP_SHOWWINDOW = 0x0040,
}

[Flags]
public enum MoveFileFlags
{
    MOVEFILE_REPLACE_EXISTING = 0x1,
    MOVEFILE_COPY_ALLOWED = 0x2,
    MOVEFILE_DELAY_UNTIL_REBOOT = 0x4,
    MOVEFILE_WRITE_THROUGH = 0x8,
    MOVEFILE_CREATE_HARDLINK = 0x10,
    MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20
}

public enum DeviceCap
{
    DRIVERVERSION = 0,
    TECHNOLOGY = 2,
    HORZSIZE = 4,
    VERTSIZE = 6,
    HORZRES = 8,
    VERTRES = 10,
    BITSPIXEL = 12,
    PLANES = 14,
    NUMBRUSHES = 16,
    NUMPENS = 18,
    NUMMARKERS = 20,
    NUMFONTS = 22,
    NUMCOLORS = 24,
    PDEVICESIZE = 26,
    CURVECAPS = 28,
    LINECAPS = 30,
    POLYGONALCAPS = 32,
    TEXTCAPS = 34,
    CLIPCAPS = 36,
    RASTERCAPS = 38,
    ASPECTX = 40,
    ASPECTY = 42,
    ASPECTXY = 44,
    SHADEBLENDCAPS = 45,
    LOGPIXELSX = 88,
    LOGPIXELSY = 90,
    SIZEPALETTE = 104,
    NUMRESERVED = 106,
    COLORRES = 108,
    PHYSICALWIDTH = 110,
    PHYSICALHEIGHT = 111,
    PHYSICALOFFSETX = 112,
    PHYSICALOFFSETY = 113,
    SCALINGFACTORX = 114,
    SCALINGFACTORY = 115,
    VREFRESH = 116,
    DESKTOPVERTRES = 117,
    DESKTOPHORZRES = 118,
    BLTALIGNMENT = 119
}

public enum MonitorOptions : uint
{
    MONITOR_DEFAULTTONULL = 0x00000000,
    MONITOR_DEFAULTTOPRIMARY = 0x00000001,
    MONITOR_DEFAULTTONEAREST = 0x00000002
}

public enum MonitorDpiType
{
    MDT_Effective_DPI = 0,
    MDT_Angular_DPI = 1,
    MDT_Raw_DPI = 2,
}
