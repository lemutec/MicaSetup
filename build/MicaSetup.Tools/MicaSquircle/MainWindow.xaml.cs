using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Design.Controls;
using MicaSquircle.Extension;
using MicaSquircle.Helpers;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Wpf.Ui.Controls;
using FontFamily = System.Drawing.FontFamily;
using FontStyleX = System.Drawing.FontStyle;

namespace MicaSquircle;

[INotifyPropertyChanged]
public partial class MainWindow : FluentWindow
{
    [ObservableProperty]
    private string? path = null;

    [ObservableProperty]
    private bool createPng = true;

    [ObservableProperty]
    private bool createIco = true;

    [ObservableProperty]
    private IconType iconType = IconType.Normal;

    partial void OnIconTypeChanged(IconType value)
    {
        if (File.Exists(Path))
        {
            CreateSquircle(Path!);
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
            CreateSquircle(Path = openFileDialog.FileName);
        }
    }

    [RelayCommand]
    private void CreateSquircle(string filename)
    {
        ImageSource = ReadBitmap(filename)
            .DrawFrame("#50FFFFFF".ToColor(), 1)
            .ToImageSource();
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
        using Bitmap bitmap = ReadBitmap();

        try
        {
            if (CreatePng)
            {
                bitmap.Save($"{pathNoExt}.png", ImageFormat.Png);
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
                bitmap.ConvertToIco($"{pathNoExt}.ico");
            }
        }
        catch
        {
            ///
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

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.FirstOrDefault() is string path)
            {
                CreateSquircle(Path = path);
            }
        }
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
