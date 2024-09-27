using CommunityToolkit.Mvvm.ComponentModel;
using MakeMui.ViewModels;
using System.ComponentModel;
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
        ViewModel.DataGrid = dataGrid;
    }
}
