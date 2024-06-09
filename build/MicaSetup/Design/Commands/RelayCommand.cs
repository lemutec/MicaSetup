using System;
using System.Runtime.CompilerServices;

namespace MicaSetup.Design.Commands;

public sealed class RelayCommand : IRelayCommand
{
    private readonly Action execute;

    private readonly Func<bool>? canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action execute)
    {
        _ = execute ?? throw new ArgumentNullException(nameof(execute));

        this.execute = execute;
    }

    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        _ = execute ?? throw new ArgumentNullException(nameof(execute));
        _ = execute ?? throw new ArgumentNullException(nameof(canExecute));

        this.execute = execute;
        this.canExecute = canExecute;
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object? parameter)
    {
        return this.canExecute?.Invoke() != false;
    }

    public void Execute(object? parameter)
    {
        this.execute();
    }
}
