using System.Linq;
using System.Windows;

namespace MakeMica.Design.Controls;

public static class MessageBoxX
{
    public static MessageBoxResult Question(string message)
    {
        return Show(null!, message, null!, MessageBoxButton.YesNo);
    }

    public static MessageBoxResult Question(string message, string title)
    {
        return Show(null!, message, title, MessageBoxButton.YesNo);
    }

    public static MessageBoxResult Show(string message)
    {
        return Show(null!, message, null!, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(string message, string title)
    {
        return Show(null!, message, title, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(string message, string title, MessageBoxButton button)
    {
        return Show(null!, message, title, button);
    }

    public static MessageBoxResult Show(Window owner, string message, string title, MessageBoxButton button)
    {
        MessageBoxWindow messageBoxWindow = new()
        {
            Owner = owner ?? GetActiveWindow(),
            Caption = title,
            Title = title,
            Message = message,
            MessageBoxButtons = button,
            WindowStartupLocation = owner is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner
        };

        _ = messageBoxWindow.ShowDialog();

        return messageBoxWindow.Result;
    }

    private static Window GetActiveWindow()
    {
        return Application.Current.Windows.Cast<Window>()
            .FirstOrDefault(window => window.IsActive && window.ShowActivated);
    }
}
