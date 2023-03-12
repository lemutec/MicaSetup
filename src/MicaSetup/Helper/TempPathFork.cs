using System;
using System.IO;

namespace MicaSetup.Helper;

public static class TempPathForkHelper
{
    public const string ForkedCli = "forked";

    public static void Fork()
    {
        if (!CommandLineHelper.Has(ForkedCli))
        {
            string tempPath = SpecialPathHelper.TempPath;

            if (!Directory.Exists(tempPath))
            {
                _ = Directory.CreateDirectory(tempPath);
            }
            string filePath = Path.Combine(tempPath, $"{(Option.Current.IsUninst ? "Uninst" : "Setup")}.exe");
            File.Copy(AppDomain.CurrentDomain.FriendlyName, filePath);
            Logger.Info($"[UseTempPathFork] Copy domain file from '{AppDomain.CurrentDomain.FriendlyName}' to '{filePath}'");
            RuntimeHelper.RestartAsElevated(filePath, tempPath, $"{RuntimeHelper.ReArguments()} /{ForkedCli}");
        }
    }

    public static void Clean()
    {
        if (CommandLineHelper.Has(ForkedCli))
        {
            string tempPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(tempPath, $"{(Option.Current.IsUninst ? "Uninst" : "Setup")}.exe");

            _ = UninstallHelper.DeleteDelayUntilReboot(tempPath);
            try
            {
                FluentProcess.Create()
                    .FileName("powershell.exe")
                    .Arguments(
                        $"""
                            Start-Sleep -s 3;
                            Remove-Item "{filePath}";
                            Remove-Item "{tempPath}";
                        """)
                    .UseShellExecute(false)
                    .CreateNoWindow()
                    .Start()
                    .Forget();
            }
            catch
            {
            }
        }
    }
}
