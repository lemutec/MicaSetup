using CommunityToolkit.Mvvm.ComponentModel;
using MakeIcon.ViewModels;
using System.ComponentModel;
using Wpf.Ui.Controls;

namespace MakeIcon.Views;

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
