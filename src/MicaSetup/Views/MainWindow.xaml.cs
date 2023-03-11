using MicaSetup.Controls;
using MicaSetup.Core;
using System;
using System.ComponentModel;

namespace MicaSetup.Views;

public partial class MainWindow : WindowX
{
    public static string SetupName => Option.Current.SetupName;

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        Closing += OnClosing;
    }

    protected override void OnActivated(EventArgs e)
    {
        if (Option.Current.BackdropMica)
        {
            this.EnableBackdrop();
        }
        base.OnActivated(e);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (Option.Current.Uninst)
        {
            if (Option.Current.Uninstalling)
            {
                e.Cancel = true;
                _ = MessageBoxX.Info(this, Mui("UninstNotCompletedTips"));
            }
        }
        else
        {
            if (Option.Current.Installing)
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
