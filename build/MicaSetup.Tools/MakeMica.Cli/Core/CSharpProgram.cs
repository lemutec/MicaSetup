using MakeMica.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MakeMica.Cli.Core;

public static class CSharpProgram
{
    public static void SetupConfig(string csPath, MicaConfig config, bool isUninst = false)
    {
        if (!File.Exists(csPath))
        {
            return;
        }

        string code = File.ReadAllText(csPath);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        if (!string.IsNullOrWhiteSpace(config.Guid))
        {
            if (Guid.TryParse(config.Guid, out _))
            {
                root = root.ReplaceAssemblyAttributeWithString("Guid", config.Guid);
            }
            else
            {
                throw new ArgumentException($"Invalid `Guid` of '{config.Guid}'.");
            }
        }

        if (!string.IsNullOrWhiteSpace(config.RequestExecutionLevel))
        {
            if (config.RequestExecutionLevel.Equals("admin", StringComparison.OrdinalIgnoreCase)
             || config.RequestExecutionLevel.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                root = root.ReplaceAssemblyAttributeWithString("RequestExecutionLevel", config.RequestExecutionLevel.ToLower());
            }
            else
            {
                throw new ArgumentException($"Invalid `RequestExecutionLevel` of '{config.RequestExecutionLevel}'.");
            }
        }

        if (config.IsEnvironmentVariable)
        {
            root = root.ReplaceOptionWithBoolean("IsEnvironmentVariable", config.IsEnvironmentVariable);
        }

        if (!string.IsNullOrWhiteSpace(config.Version))
        {
            if (Version.TryParse(config.Version, out _))
            {
                root = root.ReplaceAssemblyAttributeWithString("AssemblyVersion", config.Version);
                root = root.ReplaceAssemblyAttributeWithString("AssemblyFileVersion", config.Version);
                root = root.ReplaceOptionWithString("DisplayVersion", config.Version);
            }
            else
            {
                throw new ArgumentException($"Invalid `Version` of '{config.Version}'.");
            }
        }

        if (!string.IsNullOrWhiteSpace(config.AppName))
        {
            root = root.ReplaceOptionWithString("AppName", config.AppName);
        }

        if (!string.IsNullOrWhiteSpace(config.KeyName))
        {
            if (IsValidRegistryKeyName(config.KeyName))
            {
                root = root.ReplaceOptionWithString("KeyName", config.KeyName);
            }
            else
            {
                throw new ArgumentException($"Invalid `KeyName` of '{config.KeyName}'.");
            }
        }

        if (!string.IsNullOrWhiteSpace(config.ExeName))
        {
            root = root.ReplaceOptionWithString("ExeName", config.ExeName);

            // TODO: CHECK EXE EXITS
        }

        if (!string.IsNullOrWhiteSpace(config.Publisher))
        {
            root = root.ReplaceOptionWithString("Publisher", config.Publisher);
        }

        if (!isUninst)
        {
            if (!string.IsNullOrWhiteSpace(config.LicenseFile) || !string.IsNullOrWhiteSpace(config.License))
            {
                root = root.ReplaceOptionWithBoolean("IsUseLicenseFile", true);
            }
            else
            {
                root = root.ReplaceOptionWithBoolean("IsUseLicenseFile", false);
            }
        }

        root = root.ReplaceOptionWithBoolean("IsCreateDesktopShortcut", config.IsCreateDesktopShortcut);
        root = root.ReplaceOptionWithBoolean("IsCreateUninst", config.IsCreateUninst);
        root = root.ReplaceOptionWithBoolean("IsUninstLower", config.IsUninstLower);
        root = root.ReplaceOptionWithBoolean("IsCreateStartMenu", config.IsCreateStartMenu);
        root = root.ReplaceOptionWithBoolean("IsCreateQuickLaunch", config.IsCreateQuickLaunch);
        root = root.ReplaceOptionWithBoolean("IsCreateRegistryKeys", config.IsCreateRegistryKeys);
        root = root.ReplaceOptionWithBoolean("IsCreateAsAutoRun", config.IsCreateAsAutoRun);
        root = root.ReplaceOptionWithNullableBoolean("IsUseRegistryPreferX86", config.IsUseRegistryPreferX86);
        root = root.ReplaceOptionWithBoolean("IsAllowFirewall", config.IsAllowFirewall);
        root = root.ReplaceOptionWithBoolean("IsRefreshExplorer", config.IsRefreshExplorer);
        root = root.ReplaceOptionWithBoolean("IsInstallCertificate", config.IsInstallCertificate);
        root = root.ReplaceOptionWithBoolean("IsEnableUninstallDelayUntilReboot", config.IsEnableUninstallDelayUntilReboot);

        if (config.MessageOfPage1 != null)
        {
            root = root.ReplaceOptionWithString("MessageOfPage1", config.MessageOfPage1);
        }
        if (config.MessageOfPage2 != null)
        {
            root = root.ReplaceOptionWithString("MessageOfPage2", config.MessageOfPage2);
        }
        if (config.MessageOfPage3 != null)
        {
            root = root.ReplaceOptionWithString("MessageOfPage3", config.MessageOfPage3);
        }

        if (!isUninst)
        {
            root = root.ReplaceOptionWithBoolean("IsPinToStartMenu", config.IsPinToStartMenu);
            root = root.ReplaceOptionWithBoolean("IsCustomizeVisiableAutoRun", config.IsCustomizeVisiableAutoRun);
            root = root.ReplaceOptionWithString("AutoRunLaunchCommand", config.AutoRunLaunchCommand);
            root = root.ReplaceOptionWithBoolean("IsUseFolderPickerPreferClassic", config.IsUseFolderPickerPreferClassic);
            root = root.ReplaceOptionWithBoolean("IsUseInstallPathPreferX86", config.IsUseInstallPathPreferX86);
            root = root.ReplaceOptionWithBoolean("IsAllowFullFolderSecurity", config.IsAllowFullFolderSecurity);
            root = root.ReplaceOptionWithString("OverlayInstallRemoveExt", config.OverlayInstallRemoveExt);
            root = root.ReplaceOptionWithString("UnpackingPassword", config.UnpackingPassword);
        }

        File.Delete(csPath);
        File.WriteAllText(csPath, root.ToString());
    }

    public static bool IsValidRegistryKeyName(string keyName)
    {
        if (string.IsNullOrEmpty(keyName) || keyName.Length > 255)
        {
            return false;
        }

        if (!Regex.IsMatch(keyName, @"^[a-zA-Z0-9_-]+$"))
        {
            return false;
        }

        if (keyName.Contains(" "))
        {
            return false;
        }

        return true;
    }
}

file static class SyntaxNodeExtensions
{
    public static CompilationUnitSyntax ReplaceAssemblyAttributeWithString(this CompilationUnitSyntax root, string name, string value)
    {
        var attributeList = root.AttributeLists
          .SelectMany(al => al.Attributes)
          .FirstOrDefault(attr => attr.Name.ToString() == name);

        if (attributeList != null)
        {
            var newArgument = SyntaxFactory.AttributeArgument(
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(value)));

            var newArgumentList = SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SeparatedList([newArgument]));

            var newAttributeNode = attributeList.WithArgumentList(newArgumentList);

            return root.ReplaceNode(attributeList, newAttributeNode);
        }

        return root;
    }

    public static CompilationUnitSyntax ReplaceOptionWithAny(this CompilationUnitSyntax root, string optionName, ExpressionSyntax newRight)
    {
        var assignmentNode = root.DescendantNodes()
            .OfType<AssignmentExpressionSyntax>()
            .FirstOrDefault(a =>
                a.Left is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == optionName);

        if (assignmentNode != null)
        {
            var newAssignment = assignmentNode.WithRight(newRight);

            return root.ReplaceNode(assignmentNode, newAssignment);
        }
        else
        {
            Console.WriteLine($"[ERR] `{optionName}` assignment not found.");
        }

        return root;
    }

    public static CompilationUnitSyntax ReplaceOptionWithString(this CompilationUnitSyntax root, string optionName, string? value)
    {
        if (value == null)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
    }

    public static CompilationUnitSyntax ReplaceOptionWithNullableBoolean(this CompilationUnitSyntax root, string optionName, bool? value)
    {
        if (value == null)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else if (value == true)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
    }

    public static CompilationUnitSyntax ReplaceOptionWithBoolean(this CompilationUnitSyntax root, string optionName, bool value)
    {
        if (value)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
    }

    [Conditional("DEBUG")]
    public static void PrintSyntaxTree(this SyntaxNode node, int indentLevel = 0)
    {
        var indent = new string(' ', indentLevel * 2);
        Console.WriteLine($"{indent}{node.Kind()}: {node}");

        foreach (var child in node.ChildNodesAndTokens())
        {
            if (child.IsNode)
            {
                PrintSyntaxTree(child.AsNode()!, indentLevel + 1);
            }
        }
    }
}
