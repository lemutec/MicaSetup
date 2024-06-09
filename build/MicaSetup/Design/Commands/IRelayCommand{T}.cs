using System.Windows.Input;

namespace MicaSetup.Design.Commands;

public interface IRelayCommand<in T> : IRelayCommand, ICommand
{
    public bool CanExecute(T? parameter);

    public void Execute(T? parameter);
}
