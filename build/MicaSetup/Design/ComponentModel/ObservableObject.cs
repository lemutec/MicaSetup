using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MicaSetup.Design.ComponentModel;

public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
{
    public static readonly bool IsINotifyPropertyChangingDisabled = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event PropertyChangingEventHandler? PropertyChanging;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _ = e ?? throw new ArgumentNullException(nameof(e));

        PropertyChanged?.Invoke(this, e);
    }

    protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        _ = e ?? throw new ArgumentNullException(nameof(e));

        PropertyChanging?.Invoke(this, e);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        if (IsINotifyPropertyChangingDisabled)
        {
            return;
        }

        OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        OnPropertyChanging(propertyName);

        field = newValue;

        OnPropertyChanged(propertyName);

        return true;
    }

    protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        _ = comparer ?? throw new ArgumentNullException(nameof(comparer));

        if (comparer.Equals(field, newValue))
        {
            return false;
        }

        OnPropertyChanging(propertyName);

        field = newValue;

        OnPropertyChanged(propertyName);

        return true;
    }

    protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        _ = callback ?? throw new ArgumentNullException(nameof(callback));

        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        OnPropertyChanging(propertyName);

        callback(newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        _ = comparer ?? throw new ArgumentNullException(nameof(comparer));
        _ = callback ?? throw new ArgumentNullException(nameof(callback));

        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        OnPropertyChanging(propertyName);

        callback(newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        _ = model ?? throw new ArgumentNullException(nameof(model));
        _ = callback ?? throw new ArgumentNullException(nameof(callback));

        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        OnPropertyChanging(propertyName);

        callback(model, newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        _ = comparer ?? throw new ArgumentNullException(nameof(comparer));
        _ = model ?? throw new ArgumentNullException(nameof(model));
        _ = callback ?? throw new ArgumentNullException(nameof(callback));

        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        OnPropertyChanging(propertyName);

        callback(model, newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    protected bool SetPropertyAndNotifyOnCompletion(ref TaskNotifier? taskNotifier, Task? newValue, [CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier(), newValue, null, propertyName);
    }

    protected bool SetPropertyAndNotifyOnCompletion(ref TaskNotifier? taskNotifier, Task? newValue, Action<Task?> callback, [CallerMemberName] string? propertyName = null)
    {
        _ = callback ?? throw new ArgumentNullException(nameof(callback));

        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier(), newValue, callback, propertyName);
    }

    protected bool SetPropertyAndNotifyOnCompletion<T>(ref TaskNotifier<T>? taskNotifier, Task<T>? newValue, [CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier<T>(), newValue, null, propertyName);
    }

    protected bool SetPropertyAndNotifyOnCompletion<T>(ref TaskNotifier<T>? taskNotifier, Task<T>? newValue, Action<Task<T>?> callback, [CallerMemberName] string? propertyName = null)
    {
        _ = callback ?? throw new ArgumentNullException(nameof(callback));

        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier<T>(), newValue, callback, propertyName);
    }

    private bool SetPropertyAndNotifyOnCompletion<TTask>(ITaskNotifier<TTask> taskNotifier, TTask? newValue, Action<TTask?>? callback, [CallerMemberName] string? propertyName = null)
        where TTask : Task
    {
        if (ReferenceEquals(taskNotifier.Task, newValue))
        {
            return false;
        }

        bool isAlreadyCompletedOrNull = newValue?.IsCompleted ?? true;

        OnPropertyChanging(propertyName);

        taskNotifier.Task = newValue;

        OnPropertyChanged(propertyName);

        if (isAlreadyCompletedOrNull)
        {
            if (callback is not null)
            {
                callback(newValue);
            }

            return true;
        }

        async void MonitorTask()
        {
            await newValue!;

            if (ReferenceEquals(taskNotifier.Task, newValue))
            {
                OnPropertyChanged(propertyName);
            }

            if (callback is not null)
            {
                callback(newValue);
            }
        }

        MonitorTask();

        return true;
    }

    private interface ITaskNotifier<TTask> where TTask : Task
    {
        TTask? Task { get; set; }
    }

    protected sealed class TaskNotifier : ITaskNotifier<Task>
    {
        internal TaskNotifier()
        {
        }

        private Task? task;

        Task? ITaskNotifier<Task>.Task
        {
            get => this.task;
            set => this.task = value;
        }

        public static implicit operator Task?(TaskNotifier? notifier)
        {
            return notifier?.task;
        }
    }

    protected sealed class TaskNotifier<T> : ITaskNotifier<Task<T>>
    {
        internal TaskNotifier()
        {
        }

        private Task<T>? task;

        Task<T>? ITaskNotifier<Task<T>>.Task
        {
            get => this.task;
            set => this.task = value;
        }

        public static implicit operator Task<T>?(TaskNotifier<T>? notifier)
        {
            return notifier?.task;
        }
    }
}
