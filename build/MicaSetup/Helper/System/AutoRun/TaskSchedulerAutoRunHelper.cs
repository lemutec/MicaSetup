// <Reference Include="System.IO.Compression" />
#if false // Not being used nowadays
using MicaSetup.Attributes;
using Microsoft.Win32.TaskScheduler;
using System;

namespace MicaSetup.Helper;

[Auth(Auth.Admin)]
public static class TaskSchedulerAutoRunHelper
{
    public static void Enable(string shortcutName, string targetPath, string arguments = null!)
    {
        try
        {
            var task = TaskService.Instance.GetTask(shortcutName + "_AUTORUN");

            if (task == null)
            {
                TaskDefinition td = TaskService.Instance.NewTask();
                td.RegistrationInfo.Description = "AUTORUN";
                BootTrigger bt = new()
                {
                    Delay = TimeSpan.FromSeconds(30)
                };
                td.Triggers.Add(bt);
                td.Actions.Add(new ExecAction(targetPath, arguments, null!));
                td.Principal.RunLevel = TaskRunLevel.Highest;
                TaskService.Instance.RootFolder.RegisterTaskDefinition(shortcutName, td);
            }
            else
            {
                Logger.Warn("Task already exists.");
            }
        }
        catch (Exception ex)
        {
            Logger.Warn(ex);
        }
    }

    public static void Disable(string shortcutName)
    {
        try
        {
            var task = TaskService.Instance.GetTask(shortcutName + "_AUTORUN");

            if (task != null)
            {
                // TODO: remove task
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public static void SetEnabled(bool enable, string shortcutName, string targetPath, string arguments = null!)
    {
        if (enable)
        {
            Enable(shortcutName, targetPath, arguments);
        }
        else
        {
            Disable(shortcutName);
        }
    }
}
#endif
