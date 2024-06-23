using CommunityToolkit.Mvvm.ComponentModel;
using MakeIcon.Design.Converters;
using MakeIcon.Shared;
using MakeIcon.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace MakeIcon.Views;

[INotifyPropertyChanged]
public partial class MainWindow : FluentWindow
{
    public MainWindowViewModel ViewModel { get; }

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

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.FirstOrDefault() is string path)
            {
                ViewModel.CreateImage(ViewModel.FilePath = path);
            }
        }
    }
}

public class IconTypeValueConverter : EnumValueConverter<IconType>
{
}
