namespace MicaSetup.Shell.Dialogs;

public enum ShellThumbnailFormatOption
{
    Default,
    ThumbnailOnly = ShellNativeMethods.SIIGBF.ThumbnailOnly,
    IconOnly = ShellNativeMethods.SIIGBF.IconOnly,
}

public enum ShellThumbnailRetrievalOption
{
    Default,
    CacheOnly = ShellNativeMethods.SIIGBF.InCacheOnly,
    MemoryOnly = ShellNativeMethods.SIIGBF.MemoryOnly,
}
