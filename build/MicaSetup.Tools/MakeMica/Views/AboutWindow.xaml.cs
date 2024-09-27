using CommunityToolkit.Mvvm.ComponentModel;
using MakeMica.ViewModels;
using System.ComponentModel;
using Wpf.Ui.Controls;

namespace MakeMica.Views;

[INotifyPropertyChanged]
public partial class AboutWindow : FluentWindow
{
    public AboutViewModel ViewModel { get; }

    public AboutWindow()
    {
        DataContext = ViewModel = new();
        InitializeComponent();
    }
}
