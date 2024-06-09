using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Animation;

namespace MicaSetup.Design.Behaviors;

public abstract class TriggerAction : Animatable, IAttachedObject
{
    private bool isHosted;

    private DependencyObject associatedObject = null!;

    private Type associatedObjectTypeConstraint = null!;

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TriggerAction), new FrameworkPropertyMetadata(true));

    public bool IsEnabled
    {
        get
        {
            return (bool)GetValue(IsEnabledProperty);
        }
        set
        {
            SetValue(IsEnabledProperty, value);
        }
    }

    protected DependencyObject AssociatedObject
    {
        get
        {
            ReadPreamble();
            return associatedObject;
        }
    }

    protected virtual Type AssociatedObjectTypeConstraint
    {
        get
        {
            ReadPreamble();
            return associatedObjectTypeConstraint;
        }
    }

    internal bool IsHosted
    {
        get
        {
            ReadPreamble();
            return isHosted;
        }
        set
        {
            WritePreamble();
            isHosted = value;
            WritePostscript();
        }
    }

    DependencyObject IAttachedObject.AssociatedObject => AssociatedObject;

    internal TriggerAction(Type associatedObjectTypeConstraint)
    {
        this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
    }

    internal void CallInvoke(object parameter)
    {
        if (IsEnabled)
        {
            Invoke(parameter);
        }
    }

    protected abstract void Invoke(object parameter);

    protected virtual void OnAttached()
    {
    }

    protected virtual void OnDetaching()
    {
    }

    protected override Freezable CreateInstanceCore()
    {
        return (Freezable)Activator.CreateInstance(GetType());
    }

    public void Attach(DependencyObject dependencyObject)
    {
        if (dependencyObject != AssociatedObject)
        {
            if (AssociatedObject != null)
            {
                throw new InvalidOperationException("CannotHostTriggerActionMultipleTimesExceptionMessage");
            }

            if (dependencyObject != null && !AssociatedObjectTypeConstraint.IsAssignableFrom(dependencyObject.GetType()))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "TypeConstraintViolatedExceptionMessage", GetType().Name, dependencyObject.GetType().Name, AssociatedObjectTypeConstraint.Name));
            }

            WritePreamble();
            associatedObject = dependencyObject!;
            WritePostscript();
            OnAttached();
        }
    }

    public void Detach()
    {
        OnDetaching();
        WritePreamble();
        associatedObject = null!;
        WritePostscript();
    }
}
