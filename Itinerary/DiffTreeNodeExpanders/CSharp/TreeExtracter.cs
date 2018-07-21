using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Itinerary.DiffTreeNodeExpanders.CSharp
{
    public static class TreeExtracter
    {
        /// <summary>
        /// Reduces a Roslyn <see cref="SyntaxTree"/> to only the nodes defined "of interest" to us.
        /// </summary>
        public static List<CodeNode> ExtractTreeOfInterest(IEnumerable<SyntaxNode> syntaxNodes, CodeNode parent = null)
        {
            var list = new List<CodeNode>();
            foreach (var syntaxNode in syntaxNodes)
            {

                var identifiers = GetUniqueIdentifiers(syntaxNode, parent); // is this something that has a unique identifier within current scope? a method, field, property?

                var kind = syntaxNode.Kind();
                var source = syntaxNode.ToString();

                var codeNode = identifiers.Any()
                    ? new IdentifiableCodeNode(kind, identifiers, source)
                    : new CodeNode(kind.ToString(), kind, source);

                var childNodes = ExtractTreeOfInterest(syntaxNode.ChildNodes().ToList(), codeNode);
                codeNode.ChildNodes = childNodes;
                list.Add(codeNode);
            }

            return list;
        }

        private static List<SyntaxKind> ParentsWithUniqueBlock = new List<SyntaxKind>()
        {
            SyntaxKind.ClassDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.NamespaceDeclaration
        };

        private static List<string> GetUniqueIdentifiers(SyntaxNode syntaxNode, CodeNode parent)
        {
            if (syntaxNode == null)
                return new List<string>();

            switch (syntaxNode)
            {
                case CompilationUnitSyntax compilationUnitSyntax:
                    return new List<string> { compilationUnitSyntax.Language }; // compilation unit is a singleton, so allow it to be treated as such
                case ClassDeclarationSyntax classDeclarationSyntax:
                    return new List<string> { classDeclarationSyntax.Identifier.ToString() };
                case StructDeclarationSyntax structDeclarationSyntax:
                    return new List<string> { structDeclarationSyntax.Identifier.ToString() };
                case EnumDeclarationSyntax enumDeclarationSyntax:
                    return new List<string> { enumDeclarationSyntax.Identifier.ToString() };
                case EnumMemberDeclarationSyntax enumMemberDeclarationSyntax:
                    return new List<string> { enumMemberDeclarationSyntax.Identifier.ToString() };
                case EventDeclarationSyntax eventDeclarationSyntax:
                    return new List<string> { eventDeclarationSyntax.Identifier.ToString() };
                case FieldDeclarationSyntax fieldDeclarationSyntax:
                    return fieldDeclarationSyntax.Declaration.Variables.Select(v => v.Identifier.ToString()).ToList();
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return new List<string> { methodDeclarationSyntax.Identifier.ToString() };
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    return new List<string> { namespaceDeclarationSyntax.Name.ToString() };
                case OperatorDeclarationSyntax operatorDeclarationSyntax:
                    return new List<string> { operatorDeclarationSyntax.OperatorToken.ToString() };
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return new List<string> { propertyDeclarationSyntax.Identifier.ToString() };
                case EventFieldDeclarationSyntax eventFieldDeclarationSyntax:
                    return eventFieldDeclarationSyntax.Declaration.Variables.Select(v => v.Identifier.ToString()).ToList();
                case BlockSyntax blockSyntax:
                    if (ParentsWithUniqueBlock.Contains(parent.Kind))
                        return new List<string> { "Body" }; // make block unique so the diff thinks it's a modify instead of an add+remove
                    break;
            }
            return new List<string>();
        }
    }
}
