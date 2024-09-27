using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace MicaSetup.Design.Controls;

public partial class ObservableWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        if (comparer.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(newValue);
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(newValue);
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(model, newValue);
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

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
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier(), newValue, callback, propertyName);
    }

    protected bool SetPropertyAndNotifyOnCompletion<T>(ref TaskNotifier<T>? taskNotifier, Task<T>? newValue, [CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier<T>(), newValue, null, propertyName);
    }

    protected bool SetPropertyAndNotifyOnCompletion<T>(ref TaskNotifier<T>? taskNotifier, Task<T>? newValue, Action<Task<T>?> callback, [CallerMemberName] string? propertyName = null)
    {
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
        taskNotifier.Task = newValue;
        OnPropertyChanged(propertyName);
        if (isAlreadyCompletedOrNull)
        {
            callback?.Invoke(newValue);

            return true;
        }

        async void MonitorTask()
        {
            await newValue!;
            if (ReferenceEquals(taskNotifier.Task, newValue))
            {
                OnPropertyChanged(propertyName);
            }

            callback?.Invoke(newValue);
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
            get => task;
            set => task = value;
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
