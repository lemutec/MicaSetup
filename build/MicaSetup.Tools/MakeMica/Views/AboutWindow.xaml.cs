﻿using CommunityToolkit.Mvvm.ComponentModel;
using MakeMica.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace MakeMica.Views;

[INotifyPropertyChanged]
public partial class AboutWindow : FluentWindow
{
    public AboutViewModel ViewModel { get; }

    public AboutWindow()
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
