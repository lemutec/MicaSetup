using CommunityToolkit.Mvvm.ComponentModel;
using ComputedConverters;
using MakeIcon.ViewModels;
using System.ComponentModel;
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
        ViewModel.DropCommand.Execute(new RelayEventParameter(sender, e));
    }
}
