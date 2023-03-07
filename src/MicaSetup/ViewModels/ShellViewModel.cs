using CommunityToolkit.Mvvm.ComponentModel;
using MicaSetup.Controls;
using System.Linq;

namespace MicaSetup.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string route = null!;

    public ShellViewModel()
    {
        Routing.CreateBuilder();
        foreach (var pageItem in ShellPageSetting.PageDict)
        {
            Routing.Register(pageItem.Key, pageItem.Value);
        }
        Routing.Build();
        Route = ShellPageSetting.PageDict.FirstOrDefault().Key;
    }
}
