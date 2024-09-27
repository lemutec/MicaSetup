using CommunityToolkit.Mvvm.ComponentModel;
using MakeMica.ViewModels;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Wpf.Ui.Controls;

namespace MakeMica.Views;

[INotifyPropertyChanged]
public partial class MainWindow : FluentWindow
{
    public MainViewModel ViewModel { get; }

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
                ViewModel.Open(path);
            }
        }
    }
}
