using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ComputedConverters;
using MakeIcon.Extension;
using MakeIcon.Helpers;
using MakeIcon.Shared;
using MakeIcon.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Violeta.Controls;

namespace MakeIcon.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IMakeIconParam
{
    [ObservableProperty]
    private string? filePath = null;

    [ObservableProperty]
    private bool isCreatePng = true;

    [ObservableProperty]
    private bool isCreateIco = true;

    [ObservableProperty]
    private IconType iconType = IconType.Normal;

    partial void OnIconTypeChanged(IconType value)
    {
        if (File.Exists(FilePath))
        {
            CreateImage(FilePath!);
        }
    }

    [ObservableProperty]
    private bool isKeepOriginal = true;

    partial void OnIsKeepOriginalChanged(bool value)
    {
        if (File.Exists(FilePath))
        {
            CreateImage(FilePath!);
        }
    }

    [ObservableProperty]
    private string changedColor = "#FFFFFF";

    partial void OnChangedColorChanged(string value)
    {
        if (File.Exists(FilePath))
        {
            CreateImage(FilePath!);
        }
    }

    [Obsolete("Only CLI")]
    public bool IsTypeNormal { get; set; } = false;

    [Obsolete("Only CLI")]
    public bool IsTypeSetup { get; set; } = false;

    [Obsolete("Only CLI")]
    public bool IsTypeUninst { get; set; } = false;

    [ObservableProperty]
    private ImageSource? imageSource = null!;

    [ObservableProperty]
    private bool isSize256 = true;

    [ObservableProperty]
    private bool isSize64 = true;

    [ObservableProperty]
    private bool isSize48 = true;

    [ObservableProperty]
    private bool isSize32 = true;

    [ObservableProperty]
    private bool isSize24 = true;

    [ObservableProperty]
    private bool isSize16 = true;

    public MainWindowViewModel()
    {
    }

    [RelayCommand]
    private void Drop(RelayEventParameter param)
    {
        (object sender, DragEventArgs e) = param.Deconstruct<DragEventArgs>();
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files
                && files.FirstOrDefault() is string path)
            {
                CreateImage(FilePath = path);
            }
        }
    }

    [RelayCommand]
    private void OpenImage()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Image files (*.png)|*.png",
            Title = "Open Image",
        };

        if (openFileDialog.ShowDialog() ?? false)
        {
            CreateImage(FilePath = openFileDialog.FileName);
        }
    }

    [RelayCommand]
    public void CreateImage(string filename)
    {
        ImageSource = ImageHelper.OpenImage(
            type: IconType,
            fontFamily: PrivateFontHelper.FontFamily,
            filename: filename,
            changedColor: IsKeepOriginal ? null : ChangedColor
        ).DrawFrame("#50FFFFFF".ToColor(), 1)
        .ToImageSource();
    }

    [RelayCommand]
    private void SaveImage()
    {
        if (!File.Exists(FilePath))
        {
            return;
        }

        if (!IsCreatePng && !IsCreateIco)
        {
            return;
        }

        if (IsCreatePng)
        {
            ImageHelper.SaveImage(
                type: IconType,
                fontFamily: PrivateFontHelper.FontFamily,
                filename: FilePath!,
                ext: ".png",
                size: null,
                changedColor: IsKeepOriginal ? null : ChangedColor
            );
        }

        if (IsCreateIco)
        {
            List<int> sizes = [];

            if (IsSize256)
            {
                sizes.Add(256);
            }
            if (IsSize64)
            {
                sizes.Add(64);
            }
            if (IsSize48)
            {
                sizes.Add(48);
            }
            if (IsSize32)
            {
                sizes.Add(32);
            }
            if (IsSize24)
            {
                sizes.Add(24);
            }
            if (IsSize16)
            {
                sizes.Add(16);
            }

            if (sizes.Count == 0)
            {
                Toast.Error("Please select the size.");
                return;
            }

            ImageHelper.SaveImage(
                type: IconType,
                fontFamily: PrivateFontHelper.FontFamily,
                filename: FilePath!,
                ext: ".ico",
                size: [.. sizes],
                changedColor: IsKeepOriginal ? null : ChangedColor
            );
        }

        Toast.Success("Create completed.");
    }

    [RelayCommand]
    private void ShowGitHub()
    {
        _ = Process.Start("https://github.com/lemutec/MicaSetup");
    }

    [RelayCommand]
    private void ShowAbout()
    {
        _ = new AboutWindow() { Owner = Application.Current.MainWindow }.ShowDialog();
    }
}
