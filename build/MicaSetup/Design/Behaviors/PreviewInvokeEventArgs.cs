using System;

namespace MicaSetup.Design.Behaviors;

public class PreviewInvokeEventArgs : EventArgs
{
    public bool Cancelling { get; set; }
}
