namespace MicaSetup.Shell.Dialogs;

public sealed class ShellSearchConnector : ShellSearchCollection
{
    internal ShellSearchConnector() => CoreHelpers.ThrowIfNotWin7();

    internal ShellSearchConnector(IShellItem2 shellItem)
        : this() => nativeShellItem = shellItem;

    public new static bool IsPlatformSupported =>
            CoreHelpers.RunningOnWin7;
}
