using System.Windows;

namespace MicaSetup.Design.Behaviors;

public sealed class TriggerCollection : AttachableCollection<TriggerBase>
{
    public TriggerCollection()
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

    internal override void ItemAdded(TriggerBase item)
    {
        if (base.AssociatedObject != null)
        {
            item.Attach(base.AssociatedObject);
        }
    }

    internal override void ItemRemoved(TriggerBase item)
    {
        if (((IAttachedObject)item).AssociatedObject != null)
        {
            item.Detach();
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new TriggerCollection();
    }
}
