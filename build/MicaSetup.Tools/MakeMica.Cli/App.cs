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

        // Extract template files
        {
            if (config.Template.Contains(MicaMacro.MicaDir))
            {
                string micadir = MicaMacro.GetMicaDir();
                config.Template = config.Template.Replace(MicaMacro.MicaDir, micadir);
            }

            string? template = MicaPath.GetFullPath(config.Template);
            string? package = MicaPath.GetFullPath(config.Package);

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
