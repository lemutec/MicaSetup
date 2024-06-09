using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MicaSetup.Design.Commands;

public sealed class RelayCommand<T> : IRelayCommand<T>, IRelayCommand, ICommand
{
    private readonly Action<T?> execute;

    private readonly Predicate<T?>? canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<T?> execute)
    {
        _ = execute ?? throw new ArgumentNullException(nameof(execute));
        this.execute = execute;
    }

    public RelayCommand(Action<T?> execute, Predicate<T?> canExecute)
    {
        _ = execute ?? throw new ArgumentNullException(nameof(execute));
        _ = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public void NotifyCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T? parameter)
    {
        return canExecute?.Invoke(parameter) ?? true;
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter == null && default(T) != null)
        {
            return false;
        }

        if (!TryGetCommandArgument(parameter, out T? result))
        {
            ThrowArgumentExceptionForInvalidCommandArgument(parameter);
        }

        return CanExecute(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Execute(T? parameter)
    {
        execute(parameter);
    }

    public void Execute(object? parameter)
    {
        if (!TryGetCommandArgument(parameter, out T? result))
        {
            ThrowArgumentExceptionForInvalidCommandArgument(parameter);
        }

        Execute(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryGetCommandArgument(object? parameter, out T? result)
    {
        if (parameter == null && default(T) == null)
        {
            result = default;
            return true;
        }

        if (parameter is T val)
        {
            result = val;
            return true;
        }

        result = default;
        return false;
    }

    internal static void ThrowArgumentExceptionForInvalidCommandArgument(object? parameter)
    {
        throw GetException(parameter);
        [MethodImpl(MethodImplOptions.NoInlining)]
        static Exception GetException(object? parameter)
        {
            if (parameter == null)
            {
                return new ArgumentException(string.Format("Parameter \"{0}\" (object) must not be null, as the command type requires an argument of type {1}.", "parameter", typeof(T)), "parameter");
            }

            return new ArgumentException(string.Format("Parameter \"{0}\" (object) cannot be of type {1}, as the command type requires an argument of type {2}.", "parameter", parameter.GetType(), typeof(T)), "parameter");
        }
    }
}
