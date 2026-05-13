using MakeMica.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Text;
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

        // Default is null so we only replace it when not null or white space
        if (!string.IsNullOrWhiteSpace(config.SingleInstanceMutex))
        {
            // Replace from `UseSingleInstance(XXX)` to `UseSingleInstance(SingleInstanceMutex)`
            root = root.ReplaceHostingMethodNameWithString("UseSingleInstance", config.SingleInstanceMutex);
        }

        // Default is true so we only replace it when false
        if (!config.IsUseTempPathFork)
        {
            // Only support for installer instead of uninstaller
            if (!csPath.EndsWith("Program.un.cs"))
            {
                // Replace from `UseTempPathFork()` to `.UseTempPathFork(false)`
                root = root.ReplaceHostingMethodNameWithBoolean("UseTempPathFork", config.IsUseTempPathFork);
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
            root = root.ReplaceOptionWithBoolean("IsUseInstallPathPreferAppDataLocalPrograms", config.IsUseInstallPathPreferAppDataLocalPrograms);
            root = root.ReplaceOptionWithBoolean("IsUseInstallPathPreferAppDataRoaming", config.IsUseInstallPathPreferAppDataRoaming);
            root = root.ReplaceOptionWithBoolean("IsAllowFullFolderSecurity", config.IsAllowFullFolderSecurity);
            root = root.ReplaceOptionWithString("OverlayInstallRemoveExt", config.OverlayInstallRemoveExt);
            root = root.ReplaceOptionWithString("UnpackingPassword", config.UnpackingPassword);
        }

        if (config.CloseApplications is { Length: > 0 })
        {
            root = root.ReplaceOptionWithCloseApplications(config.CloseApplications);
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

    public static CompilationUnitSyntax ReplaceHostingMethodNameWithString(this CompilationUnitSyntax root, string hostingMethodName, string? value)
    {
        var oldInvocation = root
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .FirstOrDefault(inv =>
                inv.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == hostingMethodName);

        if (oldInvocation == null)
            return root;

        var newArgList = SyntaxFactory.ArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value!))
                )
            )
        );

        var newInvocation = oldInvocation
            .WithArgumentList(newArgList)
            .WithTriviaFrom(oldInvocation);

        return root.ReplaceNode(oldInvocation, newInvocation);
    }

    public static CompilationUnitSyntax ReplaceHostingMethodNameWithBoolean(this CompilationUnitSyntax root, string hostingMethodName, bool value)
    {
        var oldInvocation = root
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .FirstOrDefault(inv =>
                inv.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == hostingMethodName);

        if (oldInvocation == null)
            return root;

        var newArgList = SyntaxFactory.ArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
                )
            )
        );

        var newInvocation = oldInvocation
            .WithArgumentList(newArgList)
            .WithTriviaFrom(oldInvocation);

        return root.ReplaceNode(oldInvocation, newInvocation);
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

    public static CompilationUnitSyntax ReplaceOptionWithCloseApplications(this CompilationUnitSyntax root, IEnumerable<CloseApplicationItem> items)
    {
        // Build a C# collection expression string, e.g.:
        // [
        //     new CloseApplicationInfo { Target = "QuickLook.exe", CloseMessage = true, RebootPrompt = false, TerminateProcess = true, Timeout = 5 },
        // ]
        StringBuilder sb = new();
        sb.AppendLine("[");
        foreach (CloseApplicationItem item in items)
        {
            string boolStr(bool v) => v ? "true" : "false";
            string strOrNull(string? v) => v == null ? "null!" : $"\"{v}\"";
            sb.AppendLine(
                $"    new MicaSetup.Helper.CloseApplicationInfo {{ Target = \"{item.Target}\", Description = {strOrNull(item.Description)}, WindowTitle = {strOrNull(item.WindowTitle)}, CloseMessage = {boolStr(item.CloseMessage)}, RebootPrompt = {boolStr(item.RebootPrompt)}, TerminateProcess = {boolStr(item.TerminateProcess)}, Timeout = {item.Timeout} }},");
        }
        sb.Append(']');

        ExpressionSyntax collectionExpr = SyntaxFactory.ParseExpression(sb.ToString());
        return root.ReplaceOptionWithAny("CloseApplications", collectionExpr);
    }
}
