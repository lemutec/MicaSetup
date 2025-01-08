using Flucli;
using MakeMica.Shared;
using System.Text;

namespace MakeMica.Cli.Core;

internal static class CSharpCompiler
{
    public static void Build(MicaConfig config)
    {
        string vswhere = CSharpScript.FindVSWhere();

        if (!File.Exists(vswhere))
        {
            throw new FileNotFoundException("Program vswhere.exe not found on your system. Are you missing install?");
        }

        StringBuilder stdout = new();

        vswhere
            .WithArguments("-latest -property installationPath")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout, Encoding.UTF8))
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        string installationPath = stdout.ToString().Trim();

        if (!Directory.Exists(installationPath))
        {
            throw new FileNotFoundException("Visual Studio 2022 or greater not found on your system. Are you missing install?");
        }

        string msbuild = Path.Combine(installationPath, @"MSBuild\Current\Bin\MSBuild.exe");

        if (!File.Exists(msbuild))
        {
            throw new FileNotFoundException("MSBuild not found on Visual Studio. It may be caused by version mismatch.");
        }

        Console.OutputEncoding = Encoding.UTF8;

        CliResult uninstResult = msbuild
            .WithArguments(@".dist\MicaSetup.Uninst.csproj /t:Rebuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:ImportDirectoryBuildProps=false /p:RestoreUseStaticGraphEvaluation=false /restore")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Out.WriteLine(line);
            }))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Error.WriteLine(line);
            }))
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        if (!uninstResult.IsSuccess)
        {
            throw new Exception("Build Uninst failed.");
        }

        File.Copy(@".\.dist\bin\Release\MicaSetup.exe", @".\.dist\Resources\Setups\Uninst.exe", true);

        CliResult setupResult = msbuild
            .WithArguments(@".dist\MicaSetup.csproj /t:Rebuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:ImportDirectoryBuildProps=false /p:RestoreUseStaticGraphEvaluation=false /restore")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Out.WriteLine(line);
            }))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Error.WriteLine(line);
            }))
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        if (!setupResult.IsSuccess)
        {
            throw new Exception("Build Setup failed.");
        }

        string output = config.Output;

        File.Copy(@".\.dist\bin\Release\MicaSetup.exe", output, true);

        Console.WriteLine(
            """
              ____ ___  __  __ ____  _     _____ _____ _____ ____
             / ___/ _ \|  \/  |  _ \| |   | ____|_   _| ____|  _ \
            | |  | | | | |\/| | |_) | |   |  _|   | | |  _| | | | |
            | |__| |_| | |  | |  __/| |___| |___  | | | |___| |_| |
             \____\___/|_|  |_|_|   |_____|_____| |_| |_____|____/

            """
        );

        Console.WriteLine("Output: " + output);
    }
}
