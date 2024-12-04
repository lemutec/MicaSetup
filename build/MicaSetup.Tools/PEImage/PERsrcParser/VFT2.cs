namespace PEParser.PERsrcParser;

/// <summary>
/// The function of the file.
/// </summary>
public enum VFT2 : uint
{
    /// <summary>The file contains a communications driver.</summary>
    VFT2_DRV_COMM = 0x0000000A,

    /// <summary>The file contains a display driver.</summary>
    VFT2_DRV_DISPLAY = 0x00000004,

    /// <summary>The file contains an installable driver.</summary>
    VFT2_DRV_INSTALLABLE = 0x00000008,

    /// <summary>The file contains a keyboard driver.</summary>
    VFT2_DRV_KEYBOARD = 0x00000002,

    /// <summary>The file contains a language driver.</summary>
    VFT2_DRV_LANGUAGE = 0x00000003,

    /// <summary>The file contains a mouse driver.</summary>
    VFT2_DRV_MOUSE = 0x00000005,

    /// <summary>The file contains a network driver.</summary>
    VFT2_DRV_NETWORK = 0x00000006,

    /// <summary>The file contains a printer driver.</summary>
    VFT2_DRV_PRINTER = 0x00000001,

    /// <summary>The file contains a sound driver.</summary>
    VFT2_DRV_SOUND = 0x00000009,

    /// <summary>The file contains a system driver.</summary>
    VFT2_DRV_SYSTEM = 0x00000007,

    /// <summary>The file contains a versioned printer driver.</summary>
    VFT2_DRV_VERSIONED_PRINTER = 0x0000000C,

    /// <summary>The driver type is unknown by the system.</summary>
    VFT2_UNKNOWN = 0x00000000,

    /// <summary>The file contains a raster font.</summary>
    VFT2_FONT_RASTER = 0x00000001,

    /// <summary>The file contains a TrueType font.</summary>
    VFT2_FONT_TRUETYPE = 0x00000003,

    /// <summary>The file contains a vector font.</summary>
    VFT2_FONT_VECTOR = 0x00000002,
}
