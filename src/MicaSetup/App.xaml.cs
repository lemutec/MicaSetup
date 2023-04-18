using MicaSetup.Helper;
using System.Windows;

namespace MicaSetup;

public partial class App : Application, IApp
{
    public App()
    {
        NotificationHelper.ClearNotice();
        InitializeComponent();
    }
}

public interface IApp
{
    public int Run();
}
