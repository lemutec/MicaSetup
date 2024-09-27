using CommunityToolkit.Mvvm.ComponentModel;
using MakeIcon.ViewModels;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
