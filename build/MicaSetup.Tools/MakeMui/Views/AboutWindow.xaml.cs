using CommunityToolkit.Mvvm.ComponentModel;
using MakeMui.ViewModels;
using System.ComponentModel;
using Wpf.Ui.Controls;

namespace MakeMui.Views;

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
