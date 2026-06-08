using MicaSetup.Helper;
using MicaSetup.Services;
using MicaSetup.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MicaSetup.Views;

public partial class MainPage : UserControl
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        DataContext = ViewModel = new();
        InitializeComponent();

        if (ViewModel.IsRtfLicense)
        {
            string? licenseUrl = ServiceManager.GetService<ITrService>()?.GetLicenseUriString();
            if (!string.IsNullOrEmpty(licenseUrl))
            {
                using Stream stream = ResourceHelper.GetStream(licenseUrl!);
                LicenseRtfBox.Selection.Load(stream, DataFormats.Rtf);
            }
        }
    }
}
