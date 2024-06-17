using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeIcon.Design.Converters;
using MakeIcon.Extension;
using MakeIcon.Helpers;
using MakeIcon.Shared;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Wpf.Ui.Controls;

namespace MakeIcon.Views;

[INotifyPropertyChanged]
public partial class MainWindow : FluentWindow
{
    [ObservableProperty]
    private string? filePath = null;

    [ObservableProperty]
    private bool createPng = true;

    [ObservableProperty]
    private bool createIco = true;

    [ObservableProperty]
    private IconType iconType = IconType.Normal;

    partial void OnIconTypeChanged(IconType value)
    {
        if (File.Exists(FilePath))
        {
            CreateImage(FilePath!);
        }
    }

    [ObservableProperty]
    private ImageSource? imageSource = null!;

    private readonly Notifier notifier;

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

    [RelayCommand]
    private void OpenImage()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Image files (*.png)|*.png",
            Title = "Open Image",
        };

        if (openFileDialog.ShowDialog() ?? false)
        {
            CreateImage(FilePath = openFileDialog.FileName);
        }
    }

    [RelayCommand]
    private void CreateImage(string filename)
    {
        ImageSource = ImageHelper.OpenImage(IconType, PrivateFontHelper.FontFamily, filename)
            .DrawFrame("#50FFFFFF".ToColor(), 1)
            .ToImageSource();
    }

    [RelayCommand]
    private void SaveImage()
    {
        if (!File.Exists(FilePath))
        {
            return;
        }

        if (!CreatePng && !CreateIco)
        {
            return;
        }

        if (CreatePng)
        {
            ImageHelper.SaveImage(IconType, PrivateFontHelper.FontFamily, FilePath!, ".png");
        }

        if (CreateIco)
        {
            ImageHelper.SaveImage(IconType, PrivateFontHelper.FontFamily, FilePath!, ".ico");
        }

        notifier.ShowInformation("Create completed.");
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.FirstOrDefault() is string path)
            {
                CreateImage(FilePath = path);
            }
        }
    }

    [RelayCommand]
    private void ShowGitHub()
    {
        _ = Process.Start("https://github.com/lemutec/MicaSetup");
    }

    [RelayCommand]
    private void ShowAbout()
    {
        _ = new AboutWindow() { Owner = Application.Current.MainWindow }.ShowDialog();
    }
}

public class IconTypeValueConverter : EnumValueConverter<IconType>
{
}
