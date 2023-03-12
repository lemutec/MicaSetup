using MicaSetup.Helper;
using System.Windows;

namespace MicaSetup.Controls;

public static class MessageBoxX
{
    public static WindowDialogResult Info(DependencyObject dependencyObject, string message)
    {
        Window owner = (dependencyObject is Window win ? win : Window.GetWindow(dependencyObject)) ?? UIDispatcherHelper.MainWindow;

        return new MessageBoxDialog()
        {
            Type = MessageBoxType.Info,
            Message = message,
        }.ShowDialog(owner);
    }

    public static WindowDialogResult Question(DependencyObject dependencyObject, string message)
    {
        Window owner = (dependencyObject is Window win ? win : Window.GetWindow(dependencyObject)) ?? UIDispatcherHelper.MainWindow;

        return new MessageBoxDialog()
        {
            Type = MessageBoxType.Question,
            Message = message,
        }.ShowDialog(owner);
    }
}
