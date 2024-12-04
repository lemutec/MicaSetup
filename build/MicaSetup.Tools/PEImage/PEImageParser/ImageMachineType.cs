namespace PEParser.PEImageParser;

/// <summary>
/// Specifies the values for the <see cref="ImageCoffHeader.Machine" /> property of the COFF header of a PE image file.
/// </summary>
public enum ImageMachineType : ushort
{
    /// <summary>
    /// The machine is unspecified. It is assumed to be applicable to any machine type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Specifies the Intel 386 or later processors and compatible processors.
    /// </summary>
    I386 = 0x14c,

    /// <summary>
    /// Specifies the MIPS R3000 little endian processor.
    /// </summary>
    R3000 = 0x162,

    /// <summary>
    /// Specifies the MIPS R4000 little endian processor.
    /// </summary>
    R4000 = 0x166,

    /// <summary>
    /// Specifies the MIPS R10000 little endian processor.
    /// </summary>
    R10000 = 0x168,

    /// <summary>
    /// Specifies the MIPS little endian WCE v2 processor.
    /// </summary>
    MIPSWCE2 = 0x169,

    /// <summary>
    /// Specifies the Alpha AXP processor family.
    /// </summary>
    Alpha = 0x184,

    /// <summary>
    /// Specifies the Hitachi SH-3 processor.
    /// </summary>
    SH3 = 0x1a2,

    /// <summary>
    /// Specifies the Hitachi SH3-DSP processor.
    /// </summary>
    SH3DSP = 0x1a3,

    /// <summary>
    /// Specifies the Hitachi SH-3E processor.
    /// </summary>
    SH3E = 0x1a4,

    /// <summary>
    /// Specifies the Hitachi SH-4 processor.
    /// </summary>
    SH4 = 0x1a6,

    /// <summary>
    /// Specifies the Hitachi SH-5 processor.
    /// </summary>
    SH5 = 0x1a8,

    /// <summary>
    /// Specifies the ARM little endian processor.
    /// </summary>
    ARM = 0x1c0,

    /// <summary>
    /// Specifies the ARM Thumb little endian processor.
    /// </summary>
    Thumb = 0x1c2,

    /// <summary>
    /// Specifies the ARM Thumb-2 little endian processor.
    /// </summary>
    ARMNT = 0x1c4,

    /// <summary>
    /// Specifies the Matsushita AM33 processor.
    /// </summary>
    AM33 = 0x1d3,

    /// <summary>
    /// Specifies the Power PC little endian processor.
    /// </summary>
    PowerPC = 0x1f0,

    /// <summary>
    /// Specifies the Power PC little endian with floating point support processor.
    /// </summary>
    PowerPCFP = 0x1f1,

    /// <summary>
    /// Specifies the Intel Itanium processor family.
    /// </summary>
    IA64 = 0x200,

    /// <summary>
    /// Specifies the MIPS16 processor.
    /// </summary>
    MIPS16 = 0x266,

    /// <summary>
    /// Specifies the Alpha 64-bit processor family.
    /// </summary>
    Alpha64 = 0x284,

    /// <summary>
    /// Specifies the MIPS with FPU processor.
    /// </summary>
    MIPSFPU = 0x366,

    /// <summary>
    /// Specifies the MIPS16 with FPU processor.
    /// </summary>
    MIPSFPU16 = 0x466,

    /// <summary>
    /// Specifies the Infinion TriCore processor family.
    /// </summary>
    Tricore = 0x520,

    /// <summary>
    /// Specifies the IMAGE_FILE_MACHINE_CEF (0x0CEF) constant.
    /// </summary>
    CEF = 0xcef,

    /// <summary>
    /// Specifies EFI byte code.
    /// </summary>
    EBC = 0xebc,

    /// <summary>
    /// Specifies the RISCV32 processor.
    /// </summary>
    RISCV32 = 0x5032,

    /// <summary>
    /// Specifies the RISCV64 processor.
    /// </summary>
    RISCV64 = 0x5064,

    /// <summary>
    /// Specifies the RISCV128 processor.
    /// </summary>
    RISCV128 = 0x5128,

    /// <summary>
    /// Specifies the AMD64 processor.
    /// </summary>
    AMD64 = 0x8664,

    /// <summary>
    /// Specifies the Mitsubishi M32R little endian processor.
    /// </summary>
    M32R = 0x9041,

    /// <summary>
    /// Specifies the ARM64 little endian processor.
    /// </summary>
    ARM64 = 0xaa64,

    /// <summary>
    /// Specifies the IMAGE_FILE_MACHINE_CEE (0xC0EE) constant.
    /// </summary>
    CEE = 0xc0ee
}
