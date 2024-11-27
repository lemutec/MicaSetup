using MicaSetup.Attributes;
using Microsoft.Win32;
using System;
using System.Linq;

namespace MicaSetup.Helper;

public static class EnvironmentVariableHelper
{
    /// <summary>
    /// Adds a directory to the system environment variable 'Path' if not already present.
    /// </summary>
    /// <param name="directoryPath">The directory path to add.</param>
    [Auth(Auth.Admin)]
    public static void AddDirectoryToSystemPath(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Logger.Error("The directory path is invalid.");
            return;
        }

        try
        {
            // Get the current Path variable from the registry
            const string envKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(envKey, writable: true);
            if (key == null)
            {
                Logger.Error("Unable to access system environment variables.");
                return;
            }

            // Get the current 'Path' value
            string? currentPath = key.GetValue("Path") as string;

            if (string.IsNullOrEmpty(currentPath))
            {
                Logger.Error("Failed to retrieve the current 'Path' value.");
                return;
            }

            // Check if the directory is already in the Path
            if (currentPath!.Split([';'], StringSplitOptions.RemoveEmptyEntries).Contains(directoryPath, StringComparer.OrdinalIgnoreCase))
            {
                Logger.Warning($"The directory '{directoryPath}' is already in the system Path.");
                return;
            }

            // Add the directory to the Path
            string updatedPath = currentPath + ";" + directoryPath;
            key.SetValue("Path", updatedPath);
            Logger.Information($"Successfully added '{directoryPath}' to the system Path.");
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Error("You need administrative privileges to modify the system Path.");
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred: {ex.Message}");
        }
    }

    [Auth(Auth.Admin)]
    public static void RemoveDirectoryFromSystemPath(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Logger.Error("The directory path is invalid.");
            return;
        }

        try
        {
            const string envKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(envKey, writable: true);
            if (key == null)
            {
                Logger.Error("Unable to access system environment variables.");
                return;
            }

            string? currentPath = key.GetValue("Path") as string;
            if (string.IsNullOrEmpty(currentPath))
            {
                Logger.Error("Failed to retrieve the current 'Path' value.");
                return;
            }

            var paths = currentPath!.Split([';'], StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!paths.Remove(directoryPath))
            {
                Logger.Warning($"The directory '{directoryPath}' was not found in the system Path.");
                return;
            }

            string updatedPath = string.Join(";", paths);
            key.SetValue("Path", updatedPath);
            Logger.Information($"Successfully removed '{directoryPath}' from the system Path.");
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Error("You need administrative privileges to modify the system Path.");
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds a directory to the current user's environment variable 'Path' if not already present.
    /// </summary>
    /// <param name="directoryPath">The directory path to add.</param>
    [Auth(Auth.User)]
    public static void AddDirectoryToUserPath(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Logger.Error("The directory path is invalid.");
            return;
        }

        try
        {
            // Get the current user's Path environment variable
            string? currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            // Initialize if current Path is null or empty
            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath = string.Empty;
            }

            // Check if the directory is already in the Path
            if (currentPath!.Split([';'], StringSplitOptions.RemoveEmptyEntries).Contains(directoryPath, StringComparer.OrdinalIgnoreCase))
            {
                Logger.Warning($"The directory '{directoryPath}' is already in the user's Path.");
                return;
            }

            // Add the directory to the Path
            string updatedPath = string.IsNullOrEmpty(currentPath) ? directoryPath : currentPath + ";" + directoryPath;
            Environment.SetEnvironmentVariable("Path", updatedPath, EnvironmentVariableTarget.User);

            Logger.Information($"Successfully added '{directoryPath}' to the user's Path.");
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred: {ex.Message}");
        }
    }

    [Auth(Auth.User)]
    public static void RemoveDirectoryFromUserPath(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Logger.Error("The directory path is invalid.");
            return;
        }

        try
        {
            string? currentPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(currentPath))
            {
                Logger.Error("No user Path variable found.");
                return;
            }

            var paths = currentPath.Split([';'], StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!paths.Remove(directoryPath))
            {
                Logger.Warning($"The directory '{directoryPath}' was not found in the user's Path.");
                return;
            }

            string updatedPath = string.Join(";", paths);
            Environment.SetEnvironmentVariable("Path", updatedPath, EnvironmentVariableTarget.User);
            Logger.Information($"Successfully removed '{directoryPath}' from the user's Path.");
        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred: {ex.Message}");
        }
    }
}
