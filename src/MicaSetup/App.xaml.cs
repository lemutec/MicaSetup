using MicaSetup.Helper;
using System.Windows;

namespace MicaSetup;

public partial class App : Application
{
    public App()
    {
        NotificationHelper.ClearNotice();
        InitializeComponent();
    }
}
