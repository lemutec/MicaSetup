using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace MakeIcon.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private string version = $"v{Assembly.GetCallingAssembly().GetName().Version.ToString(3)}";
}
