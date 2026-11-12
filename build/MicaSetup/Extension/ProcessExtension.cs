using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MicaSetup.Helper;

public static class ProcessExtension
{
    public static FluentProcess FileName(this FluentProcess self, string filename)
    {
        self.StartInfo.FileName = filename;
        return self;
    }

    public static FluentProcess Arguments(this FluentProcess self, string args)
    {
        self.StartInfo.Arguments = args;
        return self;
    }

    public static FluentProcess CreateNoWindow(this FluentProcess self, bool enabled = true)
    {
        self.StartInfo.CreateNoWindow = enabled;
        return self;
    }

    public static FluentProcess Environment(this FluentProcess self, params KeyValuePair<string, string>[] vars)
    {
        foreach (var v in vars)
        {
            self.StartInfo.Environment.Add(v!);
        }
        return self;
    }

    public static Process EnvironmentVariables(this FluentProcess self, params KeyValuePair<string, string>[] vars)
    {
        foreach (var v in vars)
        {
            self.StartInfo.EnvironmentVariables.Add(v.Key, v.Value);
        }
        return self;
    }

    public static FluentProcess RedirectStandardError(this FluentProcess self, bool enabled = true)
    {
        self.StartInfo.RedirectStandardError = enabled;
        return self;
    }

    public static FluentProcess RedirectStandardInput(this FluentProcess self, bool enabled = true)
    {
        self.StartInfo.RedirectStandardInput = enabled;
        return self;
    }

    public static FluentProcess RedirectStandardOutput(this FluentProcess self, bool enabled = true)
    {
        self.StartInfo.RedirectStandardOutput = enabled;
        return self;
    }

    public static FluentProcess StandardErrorEncoding(this FluentProcess self, Encoding encoding = null!)
    {
        self.StartInfo.StandardErrorEncoding = encoding ?? Encoding.Default;
        return self;
    }

    public static FluentProcess StandardOutputEncoding(this FluentProcess self, Encoding encoding = null!)
    {
        self.StartInfo.StandardOutputEncoding = encoding ?? Encoding.Default;
        return self;
    }

    public static FluentProcess BeginOutputRead(this FluentProcess self, Stream stream)
    {
        long pos = stream.Position;
        self.StandardOutput.BaseStream.CopyTo(stream);
        stream.Position = pos;
        return self;
    }

    public static FluentProcess BeginErrorRead(this FluentProcess self, Stream stream)
    {
        long pos = stream.Position;
        self.StandardOutput.BaseStream.CopyTo(stream);
        stream.Position = pos;
        return self;
    }

    public static FluentProcess UseShellExecute(this FluentProcess self, bool enabled = true)
    {
        self.StartInfo.UseShellExecute = enabled;
        return self;
    }

    public static FluentProcess Verb(this FluentProcess self, string verb)
    {
        self.StartInfo.Verb = verb;
        return self;
    }

    /// <summary>
    /// Start without inheriting the current elevated integrity level.
    /// Uses the interactive shell user token when the current process is elevated.
    /// Pair with <see cref="Verb"/>: elevate via <c>runas</c>, de-elevate via this method.
    /// </summary>
    public static FluentProcess UseUnelevated(this FluentProcess self, bool enabled = true)
    {
        self.IsUnelevated = enabled;
        return self;
    }

    public static FluentProcess WorkingDirectory(this FluentProcess self, string directory)
    {
        self.StartInfo.WorkingDirectory = directory;
        return self;
    }

    public static FluentProcess SetOutputDataReceived(this FluentProcess self, DataReceivedEventHandler handler)
    {
        self.OutputDataReceived += handler;
        return self;
    }

    public static FluentProcess SetErrorDataReceived(this FluentProcess self, DataReceivedEventHandler handler)
    {
        self.ErrorDataReceived += handler;
        return self;
    }

    [SuppressMessage("Style", "IDE0060:")]
    public static void Forget(this FluentProcess self)
    {
    }
}

public class FluentProcess : Process
{
    public bool IsUnelevated { get; set; }

    public static FluentProcess Create()
    {
        return new FluentProcess();
    }

    public new static FluentProcess Start(string fileName)
    {
        return Create()
            .FileName(fileName)
            .Start();
    }

    public new static FluentProcess Start(string fileName, string args)
    {
        return new FluentProcess()
            .FileName(fileName)
            .Arguments(args)
            .Start();
    }

    public new FluentProcess BeginErrorReadLine()
    {
        base.BeginErrorReadLine();
        return this;
    }

    public new FluentProcess BeginOutputReadLine()
    {
        base.BeginOutputReadLine();
        return this;
    }

    public new FluentProcess WaitForExit()
    {
        base.WaitForExit();
        return this;
    }

    public new FluentProcess WaitForInputIdle()
    {
        base.WaitForInputIdle();
        return this;
    }

    public new FluentProcess Start()
    {
        Logger.Information(
        $"""
            "{StartInfo.FileName}" {StartInfo.Arguments}
        """);

        if (IsUnelevated && RuntimeHelper.IsElevated && TryStartUnelevated())
        {
            return this;
        }

        base.Start();
        return this;
    }

    private bool TryStartUnelevated()
    {
        string fileName = StartInfo.FileName;
        if (string.IsNullOrEmpty(fileName))
        {
            return false;
        }

        string arguments = StartInfo.Arguments ?? string.Empty;
        string workingDirectory = StartInfo.WorkingDirectory;
        if (string.IsNullOrEmpty(workingDirectory))
        {
            workingDirectory = Path.GetDirectoryName(fileName) ?? string.Empty;
        }

        nint hShellWnd = GetShellWindow();
        if (hShellWnd == 0)
        {
            return false;
        }

        _ = GetWindowThreadProcessId(hShellWnd, out uint shellPid);
        if (shellPid == 0)
        {
            return false;
        }

        nint hShellProcess = 0;
        nint hShellToken = 0;
        nint hPrimaryToken = 0;

        try
        {
            hShellProcess = OpenProcess(PROCESS_QUERY_INFORMATION, false, shellPid);
            if (hShellProcess == 0)
            {
                return false;
            }

            if (!OpenProcessToken(hShellProcess, TOKEN_DUPLICATE, out hShellToken))
            {
                return false;
            }

            SECURITY_ATTRIBUTES sa = default;
            if (!DuplicateTokenEx(
                    hShellToken,
                    MAXIMUM_ALLOWED,
                    ref sa,
                    SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                    TOKEN_TYPE.TokenPrimary,
                    out hPrimaryToken))
            {
                return false;
            }

            string commandLine = string.IsNullOrEmpty(arguments)
                ? $"\"{fileName}\""
                : $"\"{fileName}\" {arguments}";

            STARTUPINFO si = new() { cb = Marshal.SizeOf<STARTUPINFO>() };
            if (!CreateProcessWithTokenW(
                    hPrimaryToken,
                    0,
                    null!,
                    new StringBuilder(commandLine),
                    0,
                    0,
                    string.IsNullOrEmpty(workingDirectory) ? null! : workingDirectory,
                    ref si,
                    out PROCESS_INFORMATION pi))
            {
                return false;
            }

            _ = CloseHandle(pi.hThread);
            _ = CloseHandle(pi.hProcess);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }
        finally
        {
            if (hPrimaryToken != 0)
            {
                _ = CloseHandle(hPrimaryToken);
            }
            if (hShellToken != 0)
            {
                _ = CloseHandle(hShellToken);
            }
            if (hShellProcess != 0)
            {
                _ = CloseHandle(hShellProcess);
            }
        }
    }

    private const uint PROCESS_QUERY_INFORMATION = 0x0400;
    private const uint TOKEN_DUPLICATE = 0x0002;
    private const uint MAXIMUM_ALLOWED = 0x02000000;

    private enum SECURITY_IMPERSONATION_LEVEL
    {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation,
    }

    private enum TOKEN_TYPE
    {
        TokenPrimary = 1,
        TokenImpersonation,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public nint lpSecurityDescriptor;
        public int bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public nint lpReserved2;
        public nint hStdInput;
        public nint hStdOutput;
        public nint hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public nint hProcess;
        public nint hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [DllImport("user32.dll")]
    private static extern nint GetShellWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(nint hObject);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenProcessToken(nint ProcessHandle, uint DesiredAccess, out nint TokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DuplicateTokenEx(
        nint hExistingToken,
        uint dwDesiredAccess,
        ref SECURITY_ATTRIBUTES lpTokenAttributes,
        SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
        TOKEN_TYPE TokenType,
        out nint phNewToken);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateProcessWithTokenW(
        nint hToken,
        uint dwLogonFlags,
        string lpApplicationName,
        StringBuilder lpCommandLine,
        uint dwCreationFlags,
        nint lpEnvironment,
        string lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);
}
