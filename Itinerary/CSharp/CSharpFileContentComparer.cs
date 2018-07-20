using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.DiffTree;
using Itinerary.DiffTreeBuidling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Itinerary.CSharp
{
    public class CSharpFileContentComparer
    : IFileContentComparer
    {
        public string FileExtension => ".cs";

        public List<DiffTreeNode> Parse(string leftFilename, string rightFilename)
        {
            var leftTree = GetSemanticTree(leftFilename);
            var rightTree = GetSemanticTree(rightFilename);
            return GetDiffTreeNodes(rightTree).ToList();
        }

        private static IEnumerable<DiffTreeNode> GetDiffTreeNodes(List<CodeNode> rightTree)
        {
            return rightTree.Select(n => new DiffTreeNode(n.Label, ObjectType.Other, ChangeType.Unmodified)
            {
                ChildNodes = GetDiffTreeNodes(n.ChildNodes).ToList()
            });
        }

        private static List<CodeNode> GetSemanticTree(string leftFilename)
        {
            var nodes = CSharpSyntaxTree.ParseText(File.ReadAllText(leftFilename)).GetRoot().ChildNodes();
            return BuildCodeNodes(nodes);
        }

        private static List<CodeNode> BuildCodeNodes(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var codeNodes = new List<CodeNode>();
            foreach (var syntaxNode in syntaxNodes)
            {
                if (syntaxNode.Kind() == SyntaxKind.ExpressionStatement)
                {
                    codeNodes.Add(new CodeNode(syntaxNode.ToString()));
                    continue;
                }
                var kindName = syntaxNode.Kind();
                var codeNode = new CodeNode($"{kindName} {GetLabel(syntaxNode)}");
                codeNodes.Add(codeNode);
                codeNode.ChildNodes = BuildCodeNodes(syntaxNode.ChildNodes());
            }
            return codeNodes;
        }

        private static string GetLabel(SyntaxNode syntaxNode)
        {
            switch (syntaxNode)
            {
                case ClassDeclarationSyntax classDeclarationSyntax:
                    return classDeclarationSyntax.Identifier.ToString();
                case EnumDeclarationSyntax enumDeclarationSyntax:
                    return enumDeclarationSyntax.Identifier.ToString();
                case EventDeclarationSyntax eventDeclarationSyntax:
                    return eventDeclarationSyntax.Identifier.ToString();
                case FieldDeclarationSyntax fieldDeclarationSyntax:
                    //todo
                    break;
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return methodDeclarationSyntax.Identifier.ToString();
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    return namespaceDeclarationSyntax.Name.ToString();
                case OperatorDeclarationSyntax operatorDeclarationSyntax:
                    // todo
                    break;
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return propertyDeclarationSyntax.Identifier.ToString();
                case StructDeclarationSyntax structDeclarationSyntax:
                    return structDeclarationSyntax.Identifier.ToString();
            }
            return null;
        }
    }
}
