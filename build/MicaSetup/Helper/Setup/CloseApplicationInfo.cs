namespace MicaSetup.Helper;

public class CloseApplicationInfo
{
    /// <summary>
    /// The process executable name to close, e.g. "QuickLook.exe".
    /// </summary>
    public string Target { get; set; } = string.Empty;

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
