namespace MakeIcon.Shared;

public interface IMakeIconParam
{
    public string? FilePath { get; set; }

    public bool IsCreatePng { get; set; }

    public bool IsCreateIco { get; set; }

    public bool IsTypeNormal { get; set; }

    public bool IsTypeSetup { get; set; }

    public bool IsTypeUninst { get; set; }

    public string? ChangedColor { get; set; }

    public bool IsSize256 { get; set; }

    public bool IsSize64 { get; set; }

    public bool IsSize48 { get; set; }

    public bool IsSize32 { get; set; }

    public bool IsSize24 { get; set; }

    public bool IsSize16 { get; set; }
}
