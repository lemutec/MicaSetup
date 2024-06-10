using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Vanara.InteropServices;
using Vanara.PInvoke;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MakeMica.Design.Controls;

[TemplatePart(Name = nameof(OKButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(YesButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(NoButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(CancelButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(CommandSpace), Type = typeof(Grid))]
public partial class MessageBoxWindow : FluentWindow
{
    public MessageBoxResult Result = default;

    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(object),
            typeof(MessageBoxWindow));

    public object Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(
            nameof(Message),
            typeof(object),
            typeof(MessageBoxWindow));

    public object Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public MessageBoxButton MessageBoxButtons
    {
        get => (MessageBoxButton)GetValue(MessageBoxButtonsProperty);
        set => SetValue(MessageBoxButtonsProperty, value);
    }

    public static readonly DependencyProperty MessageBoxButtonsProperty =
        DependencyProperty.Register(
            nameof(MessageBoxButtons),
            typeof(MessageBoxButton),
            typeof(MessageBoxWindow),
            new PropertyMetadata(OnMessageBoxButtonsPropertyChanged));

    private static void OnMessageBoxButtonsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((MessageBoxWindow)sender).UpdateMessageBoxButtonState();
    }

    public Button OKButton { get; protected set; } = null!;
    public Button YesButton { get; protected set; } = null!;
    public Button NoButton { get; protected set; } = null!;
    public Button CancelButton { get; protected set; } = null!;
    public Grid CommandSpace { get; protected set; } = null!;

    static MessageBoxWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBoxWindow), new FrameworkPropertyMetadata(typeof(MessageBoxWindow)));
    }

    public MessageBoxWindow()
    {
        Style = (Style)FindResource(typeof(MessageBoxWindow));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        OKButton = (Button)GetTemplateChild(nameof(OKButton));
        YesButton = (Button)GetTemplateChild(nameof(YesButton));
        NoButton = (Button)GetTemplateChild(nameof(NoButton));
        CancelButton = (Button)GetTemplateChild(nameof(CancelButton));
        CommandSpace = (Grid)GetTemplateChild(nameof(CommandSpace));

        if (GetTemplateChild("Container") is Grid container)
        {
            container.Background = new SolidColorBrush(Color.FromArgb(0x01, 0x00, 0x00, 0x00));
            container.MouseLeftButtonDown += (sender, _) =>
            {
                if (sender is UIElement ui)
                {
                    GetWindow(ui)?.DragMove();
                }
            };
        }

        UpdateMessageBoxButtonState();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (WindowBackdrop.IsSupported(WindowBackdropType.Mica))
        {
            Background = new SolidColorBrush(Colors.Transparent);
            WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        }
    }

    private void UpdateMessageBoxButtonState()
    {
        if (!IsInitialized)
        {
            return;
        }

        OKButton.Content = DialogBoxCommand.IDOK.GetString();
        YesButton.Content = DialogBoxCommand.IDYES.GetString();
        NoButton.Content = DialogBoxCommand.IDNO.GetString();
        CancelButton.Content = DialogBoxCommand.IDCANCEL.GetString();

        OKButton.Click -= OnButtonClick;
        OKButton.Click += OnButtonClick;
        YesButton.Click -= OnButtonClick;
        YesButton.Click += OnButtonClick;
        NoButton.Click -= OnButtonClick;
        NoButton.Click += OnButtonClick;
        CancelButton.Click -= OnButtonClick;
        CancelButton.Click += OnButtonClick;
        CancelButton.IsCancel = true;

        MessageBoxButton button = MessageBoxButtons;

        switch (button)
        {
            case MessageBoxButton.OK:
                OKButton!.Focus();
                OKButton!.Visibility = Visibility.Visible;
                YesButton!.Visibility = Visibility.Collapsed;
                NoButton!.Visibility = Visibility.Collapsed;
                CancelButton!.Visibility = Visibility.Collapsed;
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "FirstSpacer").First().Width = new GridLength(0d);
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "SecondaryColumn").First().Width = new GridLength(0d);
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "SecondSpacer").First().Width = new GridLength(0d);
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "CloseColumn").First().Width = new GridLength(0d);
                break;

            case MessageBoxButton.OKCancel:
                OKButton?.Focus();
                OKButton!.Visibility = Visibility.Visible;
                YesButton!.Visibility = Visibility.Collapsed;
                NoButton!.Visibility = Visibility.Collapsed;
                CancelButton!.Visibility = Visibility.Visible;
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "FirstSpacer").First().Width = new GridLength(0d);
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "SecondaryColumn").First().Width = new GridLength(0d);
                break;

            case MessageBoxButton.YesNoCancel:
                YesButton?.Focus();
                OKButton!.Visibility = Visibility.Collapsed;
                YesButton!.Visibility = Visibility.Visible;
                NoButton!.Visibility = Visibility.Visible;
                CancelButton!.Visibility = Visibility.Visible;
                break;

            case MessageBoxButton.YesNo:
                YesButton?.Focus();
                OKButton!.Visibility = Visibility.Collapsed;
                YesButton!.Visibility = Visibility.Visible;
                NoButton!.Visibility = Visibility.Visible;
                CancelButton!.Visibility = Visibility.Collapsed;
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "SecondSpacer").First().Width = new GridLength(0d);
                CommandSpace.ColumnDefinitions.Where(col => col.Name == "CloseColumn").First().Width = new GridLength(0d);
                break;

            default:
                if (OKButton != null)
                {
                    _ = OKButton.Focus();
                }
                break;
        }
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender == OKButton)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
        else if (sender == YesButton)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }
        else if (sender == NoButton)
        {
            Result = MessageBoxResult.No;
            Close();
        }
        else if (sender == CancelButton)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }
    }
}

[Obsolete("TODO")]
public enum MessageBoxIcon
{
    None,
    Info,
    Success,
    Error,
    Warning,
    Question,
}

[Obsolete("TODO")]
public enum MessageBoxSymbolGlyph
{
    Info = 0xe946,
    Error = 0xe783,
    Warning = 0xe7ba,
    Question = 0xe9ce,
    None = 0x2007,
}

file static class LocalizedDialogCommands
{
    public static string GetString(this DialogBoxCommand wBtn)
    {
        StrPtrUni strPtrUni = User32.MB_GetString((uint)wBtn);
        string src = strPtrUni.ToString()?.TrimStart('&')!;
        return Regex.Replace(src, @"\([^)]*\)", string.Empty).Replace("&", string.Empty);
    }
}

[Flags]
file enum DialogBoxCommand : int
{
    IDOK = 0,
    IDCANCEL = 1,
    IDABORT = 2,
    IDRETRY = 3,
    IDIGNORE = 4,
    IDYES = 5,
    IDNO = 6,
    IDCLOSE = 7,
    IDHELP = 8,
    IDTRYAGAIN = 9,
    IDCONTINUE = 10,
}
