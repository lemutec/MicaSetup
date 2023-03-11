using MicaSetup.Controls;
using MicaSetup.Core;
using System;
using System.ComponentModel;

namespace MicaSetup.Views;

public partial class MainWindow : WindowX
{
    public static string SetupName => Pack.Current.SetupName;

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        Closing += OnClosing;
    }

    protected override void OnActivated(EventArgs e)
    {
        if (Pack.Current.BackdropMica)
        {
            this.EnableBackdrop();
        }
        base.OnActivated(e);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (Pack.Current.Uninst)
        {
            if (Pack.Current.Uninstalling)
            {
                e.Cancel = true;
                _ = MessageBoxX.Info(this, Mui("UninstNotCompletedTips"));
            }
        }
        else
        {
            if (Pack.Current.Installing)
            {
                if (MessageBoxX.Question(this, Mui("InstNotCompletedTips")) != WindowDialogResult.Yes)
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
