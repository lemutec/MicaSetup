using MakeMica.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MakeMica.Cli.Core;

public static class CSharpProgram
{
    public static void Confirm(string csPath, MicaConfig config)
    {
        string code = File.ReadAllText(csPath);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        if (!string.IsNullOrWhiteSpace(config.Guid))
        {
            if (Guid.TryParse(config.Guid, out _))
            {
                root = root.ReplaceStringAttribute("Guid", config.Guid);
            }
            else
            {
                throw new ArgumentException($"Invalid `Guid` of '{config.Guid}'.");
            }
        }

        if (!string.IsNullOrWhiteSpace(config.KeyName))
        {
            bool isValid = IsValidRegistryKeyName(config.KeyName);

            if (!isValid)
            {
                throw new ArgumentException($"Invalid `KeyName` of '{config.KeyName}'.");
            }
            // TODO
        }

        File.WriteAllText(Path.ChangeExtension(csPath, ".X.cs"), root.ToString());
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
    public static CompilationUnitSyntax ReplaceStringAttribute(this CompilationUnitSyntax root, string name, string value)
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
