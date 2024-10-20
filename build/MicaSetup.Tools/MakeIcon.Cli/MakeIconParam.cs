using MakeIcon.Shared;

namespace MakeIcon.Cli;

public sealed class MakeIconParam : IMakeIconParam
{
    public string? FilePath { get; set; } = null;
    public bool IsCreatePng { get; set; } = true;
    public bool IsCreateIco { get; set; } = true;
    public bool IsTypeNormal { get; set; } = true;
    public bool IsTypeSetup { get; set; } = true;
    public bool IsTypeUninst { get; set; } = true;
    public string? ChangedColor { get; set; } = null;
    public bool IsSize256 { get; set; } = true;
    public bool IsSize64 { get; set; } = true;
    public bool IsSize48 { get; set; } = true;
    public bool IsSize32 { get; set; } = true;
    public bool IsSize24 { get; set; } = true;
    public bool IsSize16 { get; set; } = true;

    public static IMakeIconParam Create(IDictionary<string, string> kvp)
    {
        MakeIconParam param = new();

        if (kvp.ContainsKey("input"))
        {
            param.FilePath = kvp["input"];
        }

        if (kvp.ContainsKey("type"))
        {
            param.IsTypeNormal = kvp["type"].Contains("normal");
            param.IsTypeSetup = kvp["type"].Contains("setup");
            param.IsTypeUninst = kvp["type"].Contains("uninst");
        }

        if (kvp.ContainsKey("color"))
        {
            param.ChangedColor = kvp["color"];
        }

        if (kvp.ContainsKey("size"))
        {
            param.IsSize256 = kvp["size"].Contains("256");
            param.IsSize64 = kvp["size"].Contains("64");
            param.IsSize48 = kvp["size"].Contains("48");
            param.IsSize32 = kvp["size"].Contains("32");
            param.IsSize24 = kvp["size"].Contains("24");
            param.IsSize16 = kvp["size"].Contains("16");
        }

        if (kvp.ContainsKey("ext"))
        {
            param.IsCreatePng = kvp["ext"].Contains("png");
            param.IsCreateIco = kvp["ext"].Contains("ico");
        }

        return param;
    }
}
