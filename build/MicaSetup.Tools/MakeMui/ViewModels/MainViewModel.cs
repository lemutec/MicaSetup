using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeMui.Core;
using MakeMui.Views;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace MakeMui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private string[] Files { get; set; } = [];
    private readonly Dictionary<string, ResDict> Table = [];

    [ObservableProperty]
    public DataGrid dataGrid = null!;

    partial void OnDataGridChanged(DataGrid value)
    {
        string[] names = Files.Select((string file) => Path.GetFileNameWithoutExtension(file)).ToArray();
        value.Columns.Clear();
        value.Columns.Add(new DataGridTextColumn { Header = "Key", Binding = new Binding("Key"), Width = new DataGridLength(1d, DataGridLengthUnitType.Star), CellStyle = (Style)value.TryFindResource("CellStyle") });
        value.Columns.Add(new DataGridTextColumn { Header = "#", Binding = new Binding("Order"), Width = new DataGridLength(30d, DataGridLengthUnitType.Pixel), CellStyle = (Style)value.TryFindResource("CellStyle") });

        foreach (string name in names)
        {
            value.Columns.Add(new DataGridTextColumn { Header = name, Binding = new Binding(name), Width = new DataGridLength(1d, DataGridLengthUnitType.Star), CellStyle = (Style)value.TryFindResource("CellStyle") });
        }

        HashSet<string> keys = [];

        foreach (string name in names)
        {
            ResDict resDict = Table[name];

            foreach (Res res in resDict.Resources)
            {
                keys.Add(res.Key);
            }
        }

        foreach (string key in keys)
        {
            Type dynamicType = DynamicClassGenerator.GenerateClass(names);
            dynamic instance = Activator.CreateInstance(dynamicType);

            instance.Key = key;

            foreach (string name in names)
            {
                PropertyInfo prop = dynamicType.GetProperty(name);
                ResDict resDict = Table[name];

                if (prop != null)
                {
                    prop.SetValue(instance, resDict.Resources.Where(res => res.Key == key).FirstOrDefault()?.Value ?? string.Empty);
                }
            }

            value.Items.Add(instance);
        }
    }

    [ObservableProperty]
    private string folder = string.Empty;

    partial void OnFolderChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        Files = Directory.GetFiles(value, "*.xaml");

        Table.Clear();
        foreach (string file in Files)
        {
            XElement xaml = XElement.Load(file);
            string name = Path.GetFileNameWithoutExtension(file);

            if (xaml.Name.LocalName != "ResourceDictionary")
            {
                continue;
            }

            ResDict resDict = ResourceDictionaryParser.ParseResourceDictionary(xaml);
            Table.Add(name, resDict);
        }
    }

    public MainViewModel()
    {
#if DEBUG
        string folder = Path.GetFullPath(@"..\..\..\..\MicaSetup\Resources\Languages");

        if (Directory.Exists(folder))
        {
            Folder = folder;
        }
#endif
    }

    [RelayCommand]
    private void OpenFolder()
    {
        // TODO
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
