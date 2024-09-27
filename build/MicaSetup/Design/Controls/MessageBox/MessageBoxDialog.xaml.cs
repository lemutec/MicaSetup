using MicaSetup.Design.Commands;
using MicaSetup.Design.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace MicaSetup.Design.Controls;

[INotifyPropertyChanged]
public partial class MessageBoxDialog : ObservableWindow
{
    [ObservableProperty]
    private string message = null!;

    [ObservableProperty]
    protected bool okayVisiable = true;

    [ObservableProperty]
    protected bool yesVisiable = false;

    [ObservableProperty]
    protected bool noVisiable = false;

    [ObservableProperty]
    protected WindowDialogResult result = WindowDialogResult.None;

    partial void OnResultChanged(WindowDialogResult value)
    {
        _ = value;
        Close();
    }

    [ObservableProperty]
    private string iconString = "\xe915";

    [ObservableProperty]
    private MessageBoxType type = MessageBoxType.Info;

    partial void OnTypeChanged(MessageBoxType value)
    {
        IconString = value switch
        {
            MessageBoxType.Question => "\xe918",
            MessageBoxType.Info or _ => "\xe915",
        };

        OkayVisiable = value switch
        {
            MessageBoxType.Question => false,
            MessageBoxType.Info or _ => true,
        };

        YesVisiable = value switch
        {
            MessageBoxType.Question => true,
            MessageBoxType.Info or _ => false,
        };

        NoVisiable = value switch
        {
            MessageBoxType.Question => true,
            MessageBoxType.Info or _ => false,
        };
    }

    public MessageBoxDialog()
    {
        DataContext = this;
        InitializeComponent();
    }

    [RelayCommand]
    public void Okay()
    {
        Result = WindowDialogResult.OK;
    }

    [RelayCommand]
    public void Yes()
    {
        Result = WindowDialogResult.Yes;
    }

    [RelayCommand]
    public void No()
    {
        Result = WindowDialogResult.No;
    }

    public WindowDialogResult ShowDialog(Window owner)
    {
        Owner = owner;
        ShowDialog();
        return Result;
    }
}

public enum MessageBoxType
{
    Info,
    Question,
}

public enum WindowDialogResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Yes = 6,
    No = 7
}

partial class MessageBoxDialog
{
    public string Message
    {
        get => message;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(message, value))
            {
                OnMessageChanging(value);
                OnMessageChanging(default, value);
                message = value;
                OnMessageChanged(value);
                OnMessageChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Message"));
            }
        }
    }

    public bool OkayVisiable
    {
        get => okayVisiable;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(okayVisiable, value))
            {
                OnOkayVisiableChanging(value);
                OnOkayVisiableChanging(default, value);
                okayVisiable = value;
                OnOkayVisiableChanged(value);
                OnOkayVisiableChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("OkayVisiable"));
            }
        }
    }

    public bool YesVisiable
    {
        get => yesVisiable;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(yesVisiable, value))
            {
                OnYesVisiableChanging(value);
                OnYesVisiableChanging(default, value);
                yesVisiable = value;
                OnYesVisiableChanged(value);
                OnYesVisiableChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("YesVisiable"));
            }
        }
    }

    public bool NoVisiable
    {
        get => noVisiable;
        set
        {
            if (!EqualityComparer<bool>.Default.Equals(noVisiable, value))
            {
                OnNoVisiableChanging(value);
                OnNoVisiableChanging(default, value);
                noVisiable = value;
                OnNoVisiableChanged(value);
                OnNoVisiableChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("NoVisiable"));
            }
        }
    }

    public WindowDialogResult Result
    {
        get => result;
        set
        {
            if (!EqualityComparer<WindowDialogResult>.Default.Equals(result, value))
            {
                OnResultChanging(value);
                OnResultChanging(default, value);
                result = value;
                OnResultChanged(value);
                OnResultChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Result"));
            }
        }
    }

    public string IconString
    {
        get => iconString;
        set
        {
            if (!EqualityComparer<string>.Default.Equals(iconString, value))
            {
                OnIconStringChanging(value);
                OnIconStringChanging(default, value);
                iconString = value;
                OnIconStringChanged(value);
                OnIconStringChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("IconString"));
            }
        }
    }

    public MessageBoxType Type
    {
        get => @type;
        set
        {
            if (!EqualityComparer<MessageBoxType>.Default.Equals(@type, value))
            {
                OnTypeChanging(value);
                OnTypeChanging(default, value);
                @type = value;
                OnTypeChanged(value);
                OnTypeChanged(default, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Type"));
            }
        }
    }

    partial void OnMessageChanging(string value);

    partial void OnMessageChanging(string? oldValue, string newValue);

    partial void OnMessageChanged(string value);

    partial void OnMessageChanged(string? oldValue, string newValue);

    partial void OnOkayVisiableChanging(bool value);

    partial void OnOkayVisiableChanging(bool oldValue, bool newValue);

    partial void OnOkayVisiableChanged(bool value);

    partial void OnOkayVisiableChanged(bool oldValue, bool newValue);

    partial void OnYesVisiableChanging(bool value);

    partial void OnYesVisiableChanging(bool oldValue, bool newValue);

    partial void OnYesVisiableChanged(bool value);

    partial void OnYesVisiableChanged(bool oldValue, bool newValue);

    partial void OnNoVisiableChanging(bool value);

    partial void OnNoVisiableChanging(bool oldValue, bool newValue);

    partial void OnNoVisiableChanged(bool value);

    partial void OnNoVisiableChanged(bool oldValue, bool newValue);

    partial void OnResultChanging(WindowDialogResult value);

    partial void OnResultChanging(WindowDialogResult oldValue, WindowDialogResult newValue);

    partial void OnResultChanged(WindowDialogResult value);

    partial void OnResultChanged(WindowDialogResult oldValue, WindowDialogResult newValue);

    partial void OnIconStringChanging(string value);

    partial void OnIconStringChanging(string? oldValue, string newValue);

    partial void OnIconStringChanged(string value);

    partial void OnIconStringChanged(string? oldValue, string newValue);

    partial void OnTypeChanging(MessageBoxType value);

    partial void OnTypeChanging(MessageBoxType oldValue, MessageBoxType newValue);

    partial void OnTypeChanged(MessageBoxType value);

    partial void OnTypeChanged(MessageBoxType oldValue, MessageBoxType newValue);
}

partial class MessageBoxDialog
{
    private RelayCommand? okayCommand;
    public IRelayCommand OkayCommand => okayCommand ??= new RelayCommand(Okay);

    private RelayCommand? yesCommand;
    public IRelayCommand YesCommand => yesCommand ??= new RelayCommand(Yes);

    private RelayCommand? noCommand;
    public IRelayCommand NoCommand => noCommand ??= new RelayCommand(No);
}
