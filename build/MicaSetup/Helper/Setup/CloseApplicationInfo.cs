namespace MicaSetup.Helper;

public class CloseApplicationInfo
{
    /// <summary>
    /// The process executable name to close, e.g. "QuickLook.exe".
    /// </summary>
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable label shown in logs/UI when the application is running.
    /// Equivalent to WiX CloseApplication Description (used with PromptToContinue).
    /// Not used for process matching.
    /// </summary>
    public string? Description { get; set; } = null;

    /// <summary>
    /// Optional window title filter. When set, only processes whose
    /// <see cref="System.Diagnostics.Process.MainWindowTitle"/> contains this string
    /// (case-insensitive) will be closed. Useful when multiple processes share the
    /// same executable name. In WPF this corresponds to <c>Window.Title</c>.
    /// </summary>
    public string? WindowTitle { get; set; } = null;

    /// <summary>
    /// When true, sends a close message (WM_CLOSE) to the main window before terminating.
    /// Equivalent to WiX CloseApplication CloseMessage="yes".
    /// </summary>
    public bool CloseMessage { get; set; } = true;

    /// <summary>
    /// Reserved for future use. When true, could prompt for reboot if the process cannot be closed.
    /// Equivalent to WiX CloseApplication RebootPrompt="yes".
    /// </summary>
    public bool RebootPrompt { get; set; } = false;

    /// <summary>
    /// When true, forcibly terminates the process if it does not exit within <see cref="Timeout"/> seconds.
    /// Equivalent to WiX CloseApplication TerminateProcess="1".
    /// </summary>
    public bool TerminateProcess { get; set; } = true;

    /// <summary>
    /// Seconds to wait for a graceful shutdown before forcibly terminating (when <see cref="TerminateProcess"/> is true).
    /// </summary>
    public int Timeout { get; set; } = 5;
}
