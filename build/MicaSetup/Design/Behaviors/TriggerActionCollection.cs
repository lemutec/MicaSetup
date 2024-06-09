using System;
using System.Windows;

namespace MicaSetup.Design.Behaviors;

public class TriggerActionCollection : AttachableCollection<TriggerAction>
{
    internal TriggerActionCollection()
    {
    }

    protected override void OnAttached()
    {
        using Enumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Attach(base.AssociatedObject);
        }
    }

    protected override void OnDetaching()
    {
        using Enumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Detach();
        }
    }

    internal override void ItemAdded(TriggerAction item)
    {
        if (item.IsHosted)
        {
            throw new InvalidOperationException("CannotHostTriggerActionMultipleTimesExceptionMessage");
        }

        if (base.AssociatedObject != null)
        {
            item.Attach(base.AssociatedObject);
        }

        item.IsHosted = true;
    }

    internal override void ItemRemoved(TriggerAction item)
    {
        if (((IAttachedObject)item).AssociatedObject != null)
        {
            item.Detach();
        }

        item.IsHosted = false;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new TriggerActionCollection();
    }
}
