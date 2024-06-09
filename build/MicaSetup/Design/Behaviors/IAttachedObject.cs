using System.Windows;

namespace MicaSetup.Design.Behaviors;

public interface IAttachedObject
{
    public DependencyObject AssociatedObject { get; }

    public void Attach(DependencyObject dependencyObject);

    public void Detach();
}
