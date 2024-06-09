using System;
using System.Windows;

namespace MicaSetup.Design.Behaviors;

public static class Interaction
{
    private static readonly DependencyProperty TriggersProperty = DependencyProperty.RegisterAttached("ShadowTriggers", typeof(TriggerCollection), typeof(Interaction), new FrameworkPropertyMetadata(OnTriggersChanged));

    private static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("ShadowBehaviors", typeof(BehaviorCollection), typeof(Interaction), new FrameworkPropertyMetadata(OnBehaviorsChanged));

    internal static bool ShouldRunInDesignMode { get; set; }

    public static TriggerCollection GetTriggers(DependencyObject obj)
    {
        TriggerCollection triggerCollection = (TriggerCollection)obj.GetValue(TriggersProperty);
        if (triggerCollection == null)
        {
            triggerCollection = new TriggerCollection();
            obj.SetValue(TriggersProperty, triggerCollection);
        }

        return triggerCollection;
    }

    public static BehaviorCollection GetBehaviors(DependencyObject obj)
    {
        BehaviorCollection behaviorCollection = (BehaviorCollection)obj.GetValue(BehaviorsProperty);
        if (behaviorCollection == null)
        {
            behaviorCollection = [];
            obj.SetValue(BehaviorsProperty, behaviorCollection);
        }

        return behaviorCollection;
    }

    private static void OnBehaviorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        BehaviorCollection behaviorCollection = (BehaviorCollection)args.OldValue;
        BehaviorCollection behaviorCollection2 = (BehaviorCollection)args.NewValue;
        if (behaviorCollection == behaviorCollection2)
        {
            return;
        }

        if (behaviorCollection != null && ((IAttachedObject)behaviorCollection).AssociatedObject != null)
        {
            behaviorCollection.Detach();
        }

        if (behaviorCollection2 != null && obj != null)
        {
            if (((IAttachedObject)behaviorCollection2).AssociatedObject != null)
            {
                throw new InvalidOperationException("CannotHostBehaviorCollectionMultipleTimesExceptionMessage");
            }

            behaviorCollection2.Attach(obj);
        }
    }

    private static void OnTriggersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        TriggerCollection? triggerCollection = args.OldValue as TriggerCollection;
        TriggerCollection? triggerCollection2 = args.NewValue as TriggerCollection;
        if (triggerCollection == triggerCollection2)
        {
            return;
        }

        if (triggerCollection != null && ((IAttachedObject)triggerCollection).AssociatedObject != null)
        {
            triggerCollection.Detach();
        }

        if (triggerCollection2 != null && obj != null)
        {
            if (((IAttachedObject)triggerCollection2).AssociatedObject != null)
            {
                throw new InvalidOperationException("CannotHostTriggerCollectionMultipleTimesExceptionMessage");
            }

            triggerCollection2.Attach(obj);
        }
    }

    internal static bool IsElementLoaded(FrameworkElement element)
    {
        return element.IsLoaded;
    }
}
