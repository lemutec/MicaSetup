using MakeMica.Cli.Core;
using MakeMica.Cli.Helper;
using MakeMica.Shared;
using PureSharpCompress.Common;

namespace MakeMica.Cli;

internal sealed class App
{
    public void Run(MicaConfig config = null!)
    {
        _ = config ?? throw new ArgumentNullException(nameof(config));

        // Solve Marco, CAN'T change the order.
        {
            config.Template = config.Template.SolveTemplate();
            config.Version = config.Version.SolveVersion(config, PEImageHelper.GetFileVersionFromArchive!);
            config.Output = config.Output.SolveOutput(config);
            config.Favicon = config.Favicon?.SolveIcon();
            config.Icon = config.Icon?.SolveIcon();
            config.UnIcon = config.UnIcon?.SolveIcon();
        }

        // Extract template files.
        {
            string? template = MicaMacro.GetFullPath(config.Template);
            string? package = MicaMacro.GetFullPath(config.Package);

            if (!File.Exists(template))
            {
                Console.WriteLine($"ERR: Template file '{template}' not found.");
                return;
            }

            if (!File.Exists(package))
            {
                Console.WriteLine($"ERR: Package file '{package}' not found.");
                return;
            }

            if (Directory.Exists(".dist"))
            {
                Directory.Delete(".dist", true);
            }
            _ = Directory.CreateDirectory(".dist");

            ArchiveFileHelper.ExtractAll(".dist", template, options: new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true,
                PreserveAttributes = false,
                PreserveFileTime = true,
            });
        }

        // Apply your config
        {
            CSharpProject.SetupConfig(@".dist\MicaSetup.csproj", config, isUninst: false);
            CSharpProject.SetupConfig(@".dist\MicaSetup.Uninst.csproj", config, isUninst: true);
            CSharpProgram.SetupConfig(@".dist\Program.cs", config, isUninst: false);
            CSharpProgram.SetupConfig(@".dist\Program.un.cs", config, isUninst: true);
            CSharpResource.SetupConfig(@".dist\Resources", config);
        }

        // Compile and pack the setup
        CSharpCompiler.Build(config);
    }
}
