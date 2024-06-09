using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeMica.Design.Helpers;
using System.Windows;

namespace MakeMica.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string code = string.Empty;

    public MainViewModel()
    {
        Code = ResourceHelper.GetString(@"pack://application:,,,/Resources/Templates/Sample.json");
    }

    [RelayCommand]
    private void New()
    {
        // TODO
    }

    [RelayCommand]
    private void Open()
    {
        // TODO
    }

    [RelayCommand]
    private void Save()
    {
        // TODO
    }

    [RelayCommand]
    private void SaveAs()
    {
        // TODO
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand]
    private void Build()
    {
        // TODO
    }

    [RelayCommand]
    private void OpenFolder()
    {
        // TODO
    }

    [RelayCommand]
    private void Beautify()
    {
        // TODO
    }
}
