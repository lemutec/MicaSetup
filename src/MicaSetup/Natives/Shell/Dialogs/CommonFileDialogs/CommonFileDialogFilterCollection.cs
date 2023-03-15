using System.Collections.ObjectModel;

namespace MicaSetup.Shell.Dialogs;

public class CommonFileDialogFilterCollection : Collection<CommonFileDialogFilter>
{
    internal CommonFileDialogFilterCollection() { }

    internal ShellNativeMethods.FilterSpec[] GetAllFilterSpecs()
    {
        var filterSpecs = new ShellNativeMethods.FilterSpec[Count];

        for (var i = 0; i < Count; i++)
        {
            filterSpecs[i] = this[i].GetFilterSpec();
        }

        return filterSpecs;
    }
}
