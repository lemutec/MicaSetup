using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeIcon.Design.Controls;
using MakeIcon.Design.Converters;
using MakeIcon.Extension;
using MakeIcon.Helpers;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Wpf.Ui.Controls;
using FontFamily = System.Drawing.FontFamily;
using FontStyleX = System.Drawing.FontStyle;

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

    private Bitmap ReadBitmap(string filename = "Favicon.png")
    {
        Bitmap bitmap = new(256, 256);
        FontFamily fontFamily = PrivateFontHelper.FontFamily;

        bitmap.AddImage(new Bitmap(filename), 0, 0, 256, 256);

        if (IconType == IconType.Setup)
        {
            // Circle
            bitmap.AddIconFont(Selection.Circle, 80, fontFamily, FontStyleX.Regular, "#EE24CDB9".ToColor(), 70, 75);

            // Up
            bitmap.AddIconFont(Selection.GallerySortReverse, 60, fontFamily, FontStyleX.Bold, "#FFFFFF".ToColor(), 70, 75);
        }
        else if (IconType == IconType.Uninst)
        {
            // Circle
            bitmap.AddIconFont(Selection.Circle, 80, fontFamily, FontStyleX.Regular, "#EEEB3B3B".ToColor(), 70, 75);

            // Close
            bitmap.AddIconFont(Selection.PublicCancelFilled, 60, fontFamily, FontStyleX.Bold, "#FFFFFF".ToColor(), 70, 75);
        }
        return bitmap;
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
        ImageSource = ReadBitmap(filename)
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

        string pathNoExt = $"Favicon{IconType switch
        {
            IconType.Setup => nameof(IconType.Setup),
            IconType.Uninst => nameof(IconType.Uninst),
            _ => string.Empty,
        }}";
        using Bitmap bitmap = ReadBitmap(FilePath!);

        try
        {
            if (CreatePng)
            {
                string path = Path.Combine(new FileInfo(FilePath).DirectoryName, $"{pathNoExt}.png");

                if (Path.GetFullPath(path) == Path.GetFullPath(FilePath))
                {
                    // Skip if the file is the same
                    return;
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                bitmap.Save(path, ImageFormat.Png);
            }
        }
        catch
        {
            ///
        }

        try
        {
            if (CreateIco)
            {
                string path = Path.Combine(new FileInfo(FilePath).DirectoryName, $"{pathNoExt}.ico");

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                bitmap.ConvertToIco(path);
            }
        }
        catch
        {
            ///
        }

        notifier.ShowInformation("Create completed.");
    }

    [Obsolete]
    [RelayCommand]
    private void Folder()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            return;
        }

        string path = Path.Combine(new FileInfo(FilePath).DirectoryName, $@"Favicon{IconType switch
        {
            IconType.Setup => nameof(IconType.Setup),
            IconType.Uninst => nameof(IconType.Uninst),
            _ => string.Empty,
        }}.{(CreatePng ? "png" : "ico")}");

        if (!File.Exists(path))
        {
            return;
        }

        _ = Process.Start("explorer.exe", $@"/e,/select,{path}");
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

public enum IconType
{
    Normal,
    Setup,
    Uninst,
}

public class IconTypeValueConverter : EnumValueConverter<IconType>
{
}
