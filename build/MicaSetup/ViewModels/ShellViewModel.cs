using MicaSetup.Design.ComponentModel;
using MicaSetup.Design.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MicaSetup.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string route = null!;

    public ShellViewModel()
    {
        Routing.RegisterRoute();
        Route = ShellPageSetting.PageDict.FirstOrDefault().Key;
    }
}

partial class ShellViewModel
{
    public string Route
    {
        get => route;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(route, value))
            {
                OnRouteChanging(value);
                OnRouteChanging(default, value);
                OnPropertyChanging(new PropertyChangingEventArgs("Route"));
                route = value;
                OnRouteChanged(value);
                OnRouteChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Route"));
            }
        }
    }

    partial void OnRouteChanging(string value);

    partial void OnRouteChanging(string? oldValue, string newValue);

    partial void OnRouteChanged(string value);

    partial void OnRouteChanged(string? oldValue, string newValue);
}
