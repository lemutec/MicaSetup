﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Controls;
using MicaSquircle.Core;
using MicaWPF.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Color = System.Drawing.Color;
using FontStyleX = System.Drawing.FontStyle;

namespace MicaSquircle;

[INotifyPropertyChanged]
public partial class MainWindow : MicaWindow
{
    [ObservableProperty]
    private bool createPng = true;

    [ObservableProperty]
    private bool createIco = true;

    [ObservableProperty]
    private IconType iconType = IconType.Normal;

    partial void OnIconTypeChanged(IconType value)
    {
        CreateSquircle();
    }

    [ObservableProperty]
    private ImageSource? imageSource = null!;

    private Notifier notifier;

    public MainWindow()
    {
        notifier = new(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomCenter,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(1),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        DataContext = this;
        InitializeComponent();
        CreateSquircle();
    }

    private Bitmap GetBitmap()
    {
        Bitmap bitmap = new(512, 512);

        if (IconType == IconType.Raw)
        {
            bitmap = new(512, 512);
            // IcoMoon
            bitmap.AddIconFont(Selection.IcoMoon, 370, PrivateFontHelper.FontFamily, FontStyleX.Regular, Color.White, 6, 31);
        }
        else
        {
            // Squircle
            bitmap.AddIconFont(Selection.Squircle, 340, PrivateFontHelper.FontFamily, FontStyleX.Regular, Color.White, 6, 28);

            // IcoMoon
            bitmap.AddIconFont(Selection.IcoMoon, 200, PrivateFontHelper.FontFamily, FontStyleX.Regular, Color.Black, 6, 20);
        }

        if (IconType == IconType.Setup)
        {
            // Circle
            bitmap.AddIconFont(Selection.Circle, 160, PrivateFontHelper.FontFamily, FontStyleX.Regular, ColorTranslator.FromHtml("#EE24CDB9"), 6 + 154 - 8, 20 + 150 - 8);

            // Up
            bitmap.AddIconFont(Selection.GallerySortReverse, 120, PrivateFontHelper.FontFamily, FontStyleX.Bold, ColorTranslator.FromHtml("#FFFFFF"), 6 + 154 - 8, 20 + 150 - 8);
        }
        else if (IconType == IconType.Uninst)
        {
            // Circle
            bitmap.AddIconFont(Selection.Circle, 160, PrivateFontHelper.FontFamily, FontStyleX.Regular, ColorTranslator.FromHtml("#EEEB3B3B"), 6 + 154 - 8, 20 + 150 - 8);

            // Close
            bitmap.AddIconFont(Selection.PublicCancelFilled, 120, PrivateFontHelper.FontFamily, FontStyleX.Bold, ColorTranslator.FromHtml("#FFFFFF"), 6 + 154 - 8, 20 + 150 - 8);
        }
        return bitmap;
    }

    [RelayCommand]
    private void CreateSquircle()
    {
        ImageSource = GetBitmap().DrawFrame(ColorTranslator.FromHtml("#50FFFFFF"), 60).ToImageSource();
    }

    [RelayCommand]
    private void SaveSquircle()
    {
        string pathNoExt = $"Favicon{IconType switch
        {
            IconType.Setup => nameof(IconType.Setup),
            IconType.Uninst => nameof(IconType.Uninst),
            IconType.Raw => nameof(IconType.Raw),
            _ => string.Empty,
        }}";
        using Bitmap bitmap = GetBitmap();

        if (CreatePng)
        {
            bitmap.Save($"{pathNoExt}.png", ImageFormat.Png);
        }
        if (CreateIco)
        {
            bitmap.ConvertToIco($"{pathNoExt}.ico");
        }

        notifier.ShowInformation("Create completed.");
    }

    [RelayCommand]
    private void Folder()
    {
        _ = Process.Start("explorer.exe", $"/e,/select,Favicon{(IconType switch
        {
            IconType.Setup => nameof(IconType.Setup),
            IconType.Uninst => nameof(IconType.Uninst),
            IconType.Raw => nameof(IconType.Raw),
            _ => string.Empty,
        })}.png");
    }
}

public enum IconType
{
    Normal,
    Setup,
    Uninst,
    Raw,
}

public class IconTypeValueConverter : EnumValueConverter<IconType>
{
}

public class EnumValueConverter<T> : IValueConverter where T : struct
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return false;
        }
        return Enum.TryParse(value.ToString(), out T enumValue) && Equals(enumValue, parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null!;
        }
        return Enum.Parse(typeof(T), parameter.ToString()!);
    }
}
