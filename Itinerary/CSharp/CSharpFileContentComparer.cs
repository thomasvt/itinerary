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

        private static List<CodeNode> GetSemanticTree(string leftFilename)
        {
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(leftFilename)).GetRoot();
            return BuildCodeNodes(root);
        }

        private static List<CodeNode> BuildCodeNodes(SyntaxNodeOrToken syntaxNodeOrToken)
        {
            var codeNodes = new List<CodeNode>();
            var childNodes = syntaxNodeOrToken.ChildNodesAndTokens().SelectMany(BuildCodeNodes).ToList();

            var syntaxNode = syntaxNodeOrToken.AsNode();
            var identifiers = GetUniqueIdentifiers(syntaxNode); // is this something that has a unique identifier within current scope? a method, field, property?
            var isNodeOfInterest = IsNodeOfInterest(syntaxNode);

            var kind = syntaxNodeOrToken.Kind();
            if (isNodeOfInterest || identifiers.Any())
            {
                var source = syntaxNodeOrToken.ToString();
                var codeNode = identifiers.Any()
                ? new CodeDeclarationNode(kind, identifiers, source)
                : new CodeNode(kind, source);
                codeNodes.Add(codeNode);
                codeNode.ChildNodes = childNodes;
            }
            else
            {
                codeNodes.AddRange(childNodes);
            }
            return codeNodes;
        }

        private static IEnumerable<DiffTreeNode> GetDiffTreeNodes(List<CodeNode> rightTree)
        {
            return rightTree.Select(n =>
            {
                var label = n is CodeDeclarationNode node
                    ? $"{n.Kind} {string.Join(", ", node.Identifiers)}"
                    : $"{n.Kind}";
                return new DiffTreeNode(label, ObjectType.Other, ChangeType.Unmodified)
                {
                    ChildNodes = GetDiffTreeNodes(n.ChildNodes).ToList()
                };
            });
        }

        private static List<string> GetUniqueIdentifiers(SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
                return new List<string>();

            switch (syntaxNode)
            {
                case ClassDeclarationSyntax classDeclarationSyntax:
                    return new List<string> { classDeclarationSyntax.Identifier.ToString() };
                case EnumDeclarationSyntax enumDeclarationSyntax:
                    return new List<string> {enumDeclarationSyntax.Identifier.ToString() };
                case EventDeclarationSyntax eventDeclarationSyntax:
                    return new List<string> {eventDeclarationSyntax.Identifier.ToString() };
                case FieldDeclarationSyntax fieldDeclarationSyntax:
                    return fieldDeclarationSyntax.Declaration.Variables.Select(v => v.Identifier.ToString()).ToList();
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return new List<string> {methodDeclarationSyntax.Identifier.ToString() };
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    return new List<string> {namespaceDeclarationSyntax.Name.ToString() };
                case OperatorDeclarationSyntax operatorDeclarationSyntax:
                    return new List<string> {operatorDeclarationSyntax.OperatorToken.ToString()};
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return new List<string> {propertyDeclarationSyntax.Identifier.ToString() };
                case StructDeclarationSyntax structDeclarationSyntax:
                    return new List<string> {structDeclarationSyntax.Identifier.ToString() };
                case EventFieldDeclarationSyntax eventFieldDeclarationSyntax:
                    return eventFieldDeclarationSyntax.Declaration.Variables.Select(v => v.Identifier.ToString()).ToList();
            }
            return new List<string>();
        }

        private static readonly List<SyntaxKind> KindsOfInterest = new List<SyntaxKind>()
        {
            SyntaxKind.CompilationUnit,
            SyntaxKind.UsingDirective,
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.ExpressionStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.DoStatement,
            SyntaxKind.SwitchStatement,
            SyntaxKind.IfStatement,
            SyntaxKind.ElseClause,
            SyntaxKind.EventDeclaration
        };

        private static bool IsNodeOfInterest(SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
                return false;
            return KindsOfInterest.Contains(syntaxNode.Kind());
        }
    }
}
