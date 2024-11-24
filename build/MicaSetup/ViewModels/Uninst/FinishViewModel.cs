using MicaSetup.Design.Commands;
using MicaSetup.Design.ComponentModel;
using MicaSetup.Helper;
using System.Windows;

namespace MicaSetup.ViewModels;

public partial class FinishViewModel : ObservableObject
{
    public string Message => Option.Current.MessageOfPage3;

    [RelayCommand]
    public void Close()
    {
        if (ApplicationDispatcherHelper.MainWindow is Window window)
        {
            SystemCommands.CloseWindow(window);
        }
    }
}

partial class FinishViewModel
{
    private RelayCommand? closeCommand;
    public IRelayCommand CloseCommand => closeCommand ??= new RelayCommand(Close);
}
