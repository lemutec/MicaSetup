using System.IO;

namespace MicaSetup.Core;

public abstract class AbstractOverlayInstallRemoveHandler
{
    public abstract bool IsEmpty { get; }

    public abstract bool ToRemove(FileInfo fileInfo);
}

public sealed class EmptyOverlayInstallRemoveHandler : AbstractOverlayInstallRemoveHandler
{
    public static AbstractOverlayInstallRemoveHandler Empty => new EmptyOverlayInstallRemoveHandler();

    public override bool IsEmpty => true;

    public override bool ToRemove(FileInfo fileInfo) => false;
}
