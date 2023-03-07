using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;

namespace MicaSetup.ViewModels;

public partial class FinishViewModel : ObservableObject
{
    [RelayCommand]
    public void Finish(Button button)
    {
        if (Window.GetWindow(button) is Window window)
        {
            SystemCommands.CloseWindow(window);
        }
    }
}
