using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Reflection;
using System.Text;

namespace MakeMica.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private string version = $"v{Assembly.GetCallingAssembly().GetAssemblyVersion()}";
}

file static class AssemblyExtensions
{
    public static string GetAssemblyVersion(this Assembly assembly)
    {
        Version version = assembly.GetName().Version;
        StringBuilder sb = new();

        sb.Append(version.Major);
        sb.Append(".");
        sb.Append(version.Minor);
        sb.Append(".");
        sb.Append(version.Build);

        return sb.ToString();
    }
}
