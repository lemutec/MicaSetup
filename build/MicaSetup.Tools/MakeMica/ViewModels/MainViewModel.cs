using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeMica.Core;
using MakeMica.Views;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Wpf.Ui.Violeta.Resources;

namespace MakeMica.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? filePath = null;

    [ObservableProperty]
    private string code = string.Empty;

    public MainViewModel()
    {
    }

    [RelayCommand]
    private void New()
    {
        if (!string.IsNullOrWhiteSpace(Code))
        {
            if (MessageBox.Question("Overwirte current codes?", "Question") != MessageBoxResult.Yes)
            {
                return;
            }
        }

        FilePath = null!;

        //Code = JsonConvert.SerializeObject(new MicaConfig());
        Code = ResourcesProvider.GetString(@"pack://application:,,,/Resources/Templates/Sample.json");
    }

    [RelayCommand]
    private void Open()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
            FilterIndex = 1,
            Multiselect = false,
            Title = "Open JSON File",
        };

        if (openFileDialog.ShowDialog() == true)
        {
            Open(openFileDialog.FileName);
        }
    }

    public void Open(string filePath)
    {
        FilePath = filePath;
        Code = File.ReadAllText(FilePath);
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
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        // TODO
    }

    [RelayCommand]
    private void Beautify()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Code))
            {
                Code = JsonBeautifier.Beautify(Code);
            }
        }
        catch (Exception e)
        {
            MessageBox.Question(e.Message, "Error");
        }
    }

    [RelayCommand]
    private void ClearLog()
    {
        // TODO
    }

    [RelayCommand]
    private void OpenHomePage()
    {
        _ = Process.Start("https://github.com/lemutec/MicaSetup");
    }

    [RelayCommand]
    private void OpenAbout()
    {
        new AboutWindow() { Owner = Application.Current.MainWindow }.ShowDialog();
    }
}
