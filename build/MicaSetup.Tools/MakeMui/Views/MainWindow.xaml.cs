﻿using CommunityToolkit.Mvvm.ComponentModel;
using MakeMui.ViewModels;
using System.ComponentModel;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace MakeMui.Views;

[INotifyPropertyChanged]
public partial class MainWindow : FluentWindow
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        DataContext = ViewModel = new();
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (WindowBackdrop.IsSupported(WindowBackdropType.Mica))
        {
            Background = new SolidColorBrush(Colors.Transparent);
            WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        }
    }
}
