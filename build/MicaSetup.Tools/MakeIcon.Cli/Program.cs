using MakeIcon.Cli;
using MakeIcon.Cli.Helpers;
using MakeIcon.Shared;
using System.CommandLine;
using System.CommandLine.Parsing;

RootCommand rootCommand =
[
    new Option<string>(
        "--input",
        description: "Your input image file path."
    ),
    new Option<string>(
        "--type",
        description: "Support icon type normal,setup,uninst."
    ),
    new Option<string>(
        "--color",
        description: "Support change color."
    ),
    new Option<string>(
        "--size",
        description: "Support size 256,128,64,48,32,24,16."
    ),
    new Option<string>(
        "--ext",
        description: "Support extension png,ico."
    )
];

rootCommand.SetHandler(ctx =>
{
    IReadOnlyList<SymbolResult> results = ctx.ParseResult.CommandResult.Children;

    if (results.Count == 0)
    {
        Console.WriteLine("Use option --help for help.");
        return;
    }

    Dictionary<string, string> kvp = results.Select(result => new KeyValuePair<string, string>(result.Symbol.Name, result.Children[0].Tokens[0].Value)).ToDictionary(pair => pair.Key, pair => pair.Value);

    Console.WriteLine($"Input: {kvp["input"]}");
    Console.WriteLine($"Type: {kvp["type"]}");
    Console.WriteLine($"Color: {kvp["color"]}");
    Console.WriteLine($"Size: {kvp["size"]}");
    Console.WriteLine($"Ext: {kvp["ext"]}");

    IMakeIconParam param = MakeIconParam.Create(kvp);

    if (string.IsNullOrEmpty(param.FilePath))
    {
        Console.WriteLine("Empty option of --input.");
        return;
    }

    if (!param.IsCreatePng && !param.IsCreateIco)
    {
        Console.WriteLine("Empty option of --ext.");
        return;
    }

    if (!param.IsSize256 && !param.IsSize64 && !param.IsSize48 && !param.IsSize32 && !param.IsSize24 && !param.IsSize16)
    {
        Console.WriteLine("Empty option of --size.");
        return;
    }

    if (!param.IsTypeNormal && !param.IsTypeSetup && !param.IsTypeUninst)
    {
        Console.WriteLine("Empty option of --type.");
        return;
    }

    if (!File.Exists(param.FilePath))
    {
        Console.WriteLine("Input file not found.");
        return;
    }

    if (param.IsCreatePng)
    {
        if (param.IsTypeNormal)
        {
            ImageHelper.SaveImage(IconType.Normal, PrivateFontHelper.FontFamily, param.FilePath!, ".png", null, param.ChangedColor);
            Console.WriteLine("Normal type png created.");
        }
        if (param.IsTypeSetup)
        {
            ImageHelper.SaveImage(IconType.Setup, PrivateFontHelper.FontFamily, param.FilePath!, ".png", null, param.ChangedColor);
            Console.WriteLine("Setup type png created.");
        }
        if (param.IsTypeUninst)
        {
            ImageHelper.SaveImage(IconType.Uninst, PrivateFontHelper.FontFamily, param.FilePath!, ".png", null, param.ChangedColor);
            Console.WriteLine("Uninst type png created.");
        }
    }

    if (param.IsCreateIco)
    {
        List<int> sizes = [];

        if (param.IsSize256)
        {
            sizes.Add(256);
        }
        if (param.IsSize64)
        {
            sizes.Add(64);
        }
        if (param.IsSize48)
        {
            sizes.Add(48);
        }
        if (param.IsSize32)
        {
            sizes.Add(32);
        }
        if (param.IsSize24)
        {
            sizes.Add(24);
        }
        if (param.IsSize16)
        {
            sizes.Add(16);
        }

        if (sizes.Count == 0)
        {
            Console.WriteLine("Please select the size.");
            return;
        }

        if (param.IsTypeNormal)
        {
            ImageHelper.SaveImage(IconType.Normal, PrivateFontHelper.FontFamily, param.FilePath!, ".ico", [.. sizes], param.ChangedColor);
            Console.WriteLine("Normal type ico created.");
        }
        if (param.IsTypeSetup)
        {
            ImageHelper.SaveImage(IconType.Setup, PrivateFontHelper.FontFamily, param.FilePath!, ".ico", [.. sizes], param.ChangedColor);
            Console.WriteLine("Setup type ico created.");
        }
        if (param.IsTypeUninst)
        {
            ImageHelper.SaveImage(IconType.Uninst, PrivateFontHelper.FontFamily, param.FilePath!, ".ico", [.. sizes], param.ChangedColor);
            Console.WriteLine("Uninst type ico created.");
        }
    }

    Console.WriteLine("Finished.");
});

rootCommand.Invoke(args);
