using MicaSetup.Design.Controls;
using MicaSetup.Helper;
using System.ComponentModel;
using System.Windows.Media;

namespace MicaSetup.Views;

public partial class MainWindow : FluentWindow
{
    public static ImageSource? Favicon => new ImageSourceConverter().ConvertFromString($"pack://application:,,,/MicaSetup;component/Resources/Images/Favicon{(Option.Current.IsUninst ? "Uninst" : "Setup")}.ico") as ImageSource;
    public static string SetupName => Option.Current.SetupName;

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        Closing += OnClosing;
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (Option.Current.IsUninst)
        {
            if (Option.Current.Uninstalling)
            {
                e.Cancel = true;
                _ = MessageBox.Info(this, "UninstNotCompletedTips".Tr());
            }
        }
        else
        {
            if (Option.Current.Installing)
            {
                if (MessageBox.Question(this, "InstNotCompletedTips".Tr()) != WindowDialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        if (!e.Cancel)
        {
            TempPathForkHelper.Clean();
        }
    }
}
