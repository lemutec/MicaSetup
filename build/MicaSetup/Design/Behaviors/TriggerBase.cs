using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MicaSetup.Design.Behaviors;

[ContentProperty("Actions")]
public abstract class TriggerBase : Animatable, IAttachedObject
{
    private DependencyObject associatedObject = null!;

    private readonly Type associatedObjectTypeConstraint = null!;

    private static readonly DependencyPropertyKey ActionsPropertyKey = DependencyProperty.RegisterReadOnly("Actions", typeof(TriggerActionCollection), typeof(TriggerBase), new FrameworkPropertyMetadata());

    public static readonly DependencyProperty ActionsProperty = ActionsPropertyKey.DependencyProperty;

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

    public TriggerActionCollection Actions => (TriggerActionCollection)GetValue(ActionsProperty);

    DependencyObject IAttachedObject.AssociatedObject => AssociatedObject;

    public event EventHandler<PreviewInvokeEventArgs>? PreviewInvoke;

    internal TriggerBase(Type associatedObjectTypeConstraint)
    {
        this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
        TriggerActionCollection value = [];
        SetValue(ActionsPropertyKey, value);
    }

    protected void InvokeActions(object parameter)
    {
        if (this.PreviewInvoke != null)
        {
            PreviewInvokeEventArgs previewInvokeEventArgs = new();
            this.PreviewInvoke(this, previewInvokeEventArgs);
            if (previewInvokeEventArgs.Cancelling)
            {
                return;
            }
        }

        foreach (TriggerAction action in Actions)
        {
            action.CallInvoke(parameter);
        }
    }

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
                throw new InvalidOperationException("CannotHostTriggerMultipleTimesExceptionMessage");
            }

            if (dependencyObject != null && !AssociatedObjectTypeConstraint.IsAssignableFrom(dependencyObject.GetType()))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "TypeConstraintViolatedExceptionMessage", GetType().Name, dependencyObject.GetType().Name, AssociatedObjectTypeConstraint.Name));
            }

            WritePreamble();
            associatedObject = dependencyObject!;
            WritePostscript();
            Actions.Attach(dependencyObject!);
            OnAttached();
        }
    }

    public void Detach()
    {
        OnDetaching();
        WritePreamble();
        associatedObject = null!;
        WritePostscript();
        Actions.Detach();
    }
}
