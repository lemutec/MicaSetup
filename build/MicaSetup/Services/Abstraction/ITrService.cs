using System.Windows.Media;

namespace MicaSetup.Services;

public interface ITrService
{
    public FontFamily GetFontFamily();

    public string GetLicenseUriString();
}
