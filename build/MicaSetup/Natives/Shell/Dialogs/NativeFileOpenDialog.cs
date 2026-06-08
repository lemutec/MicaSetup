using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace MicaSetup.Shell.Dialog;

[ComImport]
[Guid(IIDGuid.IFileOpenDialog)]
[CoClass(typeof(FileOpenDialogRCW))]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Used as a class")]
internal interface NativeFileOpenDialog : IFileOpenDialog;
