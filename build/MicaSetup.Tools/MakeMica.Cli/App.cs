﻿using MakeMica.Cli.Core;
using MakeMica.Cli.Helper;
using MakeMica.Shared;
using SharpCompress.Common;

namespace MakeMica.Cli;

internal sealed class App
{
    public void Run(MicaConfig config = null!)
    {
        _ = config ?? throw new ArgumentNullException(nameof(config));

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

        CSharpProject.SetupConfig(@".dist\MicaSetup\MicaSetup.csproj", config, isUninst: false);
        CSharpProject.SetupConfig(@".dist\MicaSetup\MicaSetup.Uninst.csproj", config, isUninst: true);
        CSharpProgram.SetupConfig(@".dist\MicaSetup\Program.cs", config, isUninst: false);
        CSharpProgram.SetupConfig(@".dist\MicaSetup\Program.un.cs", config, isUninst: true);
        CSharpResource.SetupConfig(@".dist\MicaSetup\Resources", config);
    }
}
