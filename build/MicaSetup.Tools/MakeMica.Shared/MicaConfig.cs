namespace MakeMica.Shared;

public class MicaConfig
{
    public string Version { get; set; } = "1.0.0";

    public string TargetFramework { get; set; } = "net472";

    public string Guid { get; set; } = "00000000-0000-0000-0000-000000000000";

    public string AppName { get; set; } = "MicaApp";
    public string KeyName { get; set; } = "MicaApp";

    public string ExeName { get; set; } = "MicaApp.exe";
    public string Publisher { get; set; } = "Lemutec";
}
