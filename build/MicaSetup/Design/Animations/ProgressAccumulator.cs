using MicaSetup.Design.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace MicaSetup.Controls.Animations;

public partial class ProgressAccumulator : ObservableObject, IDisposable
{
    public Action<double>? Handler;
    public DoubleEasingAnimation? Animation;

    [ObservableProperty]
    private double duration = 3000d;

    [ObservableProperty]
    private double current = 0d;

    [ObservableProperty]
    private double from = 0d;

    [ObservableProperty]
    private double to = 100d;

    private Task task = null!;
    private bool isRunning = false;
    private DateTime startTime = default;
    private double durationTime = default;

    public ProgressAccumulator(double from = 0d, double to = 100d, double duration = 3000d, Action<double> handler = null!, DoubleEasingAnimation anime = null!)
    {
        Reset(from, to, duration, handler);
        Animation = anime;
    }

    public void Dispose()
    {
        Reset();
    }

    public ProgressAccumulator Start()
    {
        isRunning = true;
        startTime = DateTime.Now;
        durationTime = default;
        task = Task.Run(Handle);
        _ = task;
        return this;
    }

    public ProgressAccumulator Stop()
    {
        isRunning = false;
        return this;
    }

    public ProgressAccumulator Reset(double from = 0d, double to = 100d, double duration = 3000d, Action<double> handler = null!)
    {
        Stop();

        Current = From = from;
        To = to;
        Duration = duration;
        Handler = handler;
        return this;
    }

    private void Handle()
    {
        while (isRunning)
        {
            if (!SpinWait.SpinUntil(() => !isRunning, 50))
            {
                Calc();
                Handler?.Invoke(Current);
            }
        }
    }

    private double Calc()
    {
        if (durationTime <= Duration)
        {
            Current = (Animation ?? DoubleEasingAnimations.EaseOutCirc).Invoke(durationTime, From, To, Duration);
            durationTime = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        }
        return Current;
    }
}

partial class ProgressAccumulator
{
    public double Duration
    {
        get => duration;
        set
        {
            if (!EqualityComparer<double>.Default.Equals(duration, value))
            {
                OnDurationChanging(value);
                OnDurationChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("Duration"));
                duration = value;
                OnDurationChanged(value);
                OnDurationChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Duration"));
            }
        }
    }

    public double Current
    {
        get => current;
        set
        {
            if (!EqualityComparer<double>.Default.Equals(current, value))
            {
                OnCurrentChanging(value);
                OnCurrentChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("Current"));
                current = value;
                OnCurrentChanged(value);
                OnCurrentChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Current"));
            }
        }
    }

    public double From
    {
        get => @from;
        set
        {
            if (!EqualityComparer<double>.Default.Equals(@from, value))
            {
                OnFromChanging(value);
                OnFromChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("From"));
                @from = value;
                OnFromChanged(value);
                OnFromChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("From"));
            }
        }
    }

    public double To
    {
        get => to;
        set
        {
            if (!EqualityComparer<double>.Default.Equals(to, value))
            {
                OnToChanging(value);
                OnToChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("To"));
                to = value;
                OnToChanged(value);
                OnToChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("To"));
            }
        }
    }

    partial void OnDurationChanging(double value);

    partial void OnDurationChanging(double oldValue, double newValue);

    partial void OnDurationChanged(double value);

    partial void OnDurationChanged(double oldValue, double newValue);

    partial void OnCurrentChanging(double value);

    partial void OnCurrentChanging(double oldValue, double newValue);

    partial void OnCurrentChanged(double value);

    partial void OnCurrentChanged(double oldValue, double newValue);

    partial void OnFromChanging(double value);

    partial void OnFromChanging(double oldValue, double newValue);

    partial void OnFromChanged(double value);

    partial void OnFromChanged(double oldValue, double newValue);

    partial void OnToChanging(double value);

    partial void OnToChanging(double oldValue, double newValue);

    partial void OnToChanged(double value);

    partial void OnToChanged(double oldValue, double newValue);
}
