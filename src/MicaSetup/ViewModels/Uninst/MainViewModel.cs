using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaSetup.Controls;
using MicaSetup.Helper;
using System.Windows;
using System.Windows.Controls;

namespace MicaSetup.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool keepMyData = Option.Current.KeepMyData;
    partial void OnKeepMyDataChanged(bool value)
    {
        Option.Current.KeepMyData = value;
        if (!value)
        {
            _ = MessageBoxX.Info(UIDispatcherHelper.MainWindow, Mui("NotKeepMyDataTips", Mui("KeepMyDataTips")));
        }
    }

    public MainViewModel()
    {
    }

    [RelayCommand]
    private void StartUninstall(Button button)
    {
        Routing.GoToNext();
    }

    [RelayCommand]
    private void CancelUninstall(Button button)
    {
        SystemCommands.CloseWindow(Window.GetWindow(button));
    }
}
