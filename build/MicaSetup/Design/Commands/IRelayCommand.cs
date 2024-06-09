using System.Windows.Input;

namespace MicaSetup.Design.Commands;

public interface IRelayCommand : ICommand
{
    void NotifyCanExecuteChanged();
}
