namespace PEParser.PERsrcParser;

/// <summary>
/// The general type of file.
/// </summary>
public enum VFT : uint
{
    /// <summary>The file type is unknown to the system.</summary>
    VFT_UNKNOWN = 0x00000000,

    /// <summary>The file contains an application.</summary>
    VFT_APP = 0x00000001,

    /// <summary>The file contains a DLL.</summary>
    VFT_DLL = 0x00000002,

    /// <summary>
    /// The file contains a device driver. If dwFileType is VFT_DRV, dwFileSubtype contains a more specific description of the driver.
    /// </summary>
    VFT_DRV = 0x00000003,

    /// <summary>
    /// The file contains a font. If dwFileType is VFT_FONT, dwFileSubtype contains a more specific description of the font file.
    /// </summary>
    VFT_FONT = 0x00000004,

    /// <summary/>
    VFT_VXD = 0x00000005,

    /// <summary>The file contains a static-link library.</summary>
    VFT_STATIC_LIB = 0x00000007,
}
