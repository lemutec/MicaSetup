using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Squircle.Core;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media;

namespace Squircle;

[INotifyPropertyChanged]
public partial class MainWindow : Window
{
    [ObservableProperty]
    private ImageSource? imageSource = null!;

    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        CreateSquircle();
    }

    private Bitmap GetBitmap()
    {
        const int margin = 0;
        Bitmap bitmap = SquircleExtension.Create(margin: margin, n: 3d, m: 33d);
        bitmap.AddIconFont("\xe900", 220, PrivateFontHelper.FontFamily, margin, margin);
        return bitmap;
    }

    [RelayCommand]
    private void CreateSquircle()
    {
        ImageSource = GetBitmap().ToImageSource();
    }

    [RelayCommand]
    private void SaveSquircle()
    {
        using Bitmap bitmap = GetBitmap();
        bitmap.Save("Squircle.png", ImageFormat.Png);
    }

    [RelayCommand]
    private void Folder()
    {
        _ = Process.Start("explorer.exe", $"/e,/select,Squircle.png");
    }
}
