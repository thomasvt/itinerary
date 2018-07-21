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

                var identifiers = GetUniqueIdentifiers(syntaxNode, parent).ToList(); // is this something that has a unique identifier within current scope? a method, field, property?

                var kind = syntaxNode.Kind();
                var source = syntaxNode.ToString();

                // having all CodeNodes try to be unique is actually better: even when duplicate items occur, the UnorderedListComparer will match them by order of appearance.
                var codeNode = new IdentifiableCodeNode(kind, identifiers.Any()
                    ? identifiers
                    : new List<string> {kind.ToString()}, source);

                var childNodes = ExtractTreeOfInterest(syntaxNode.ChildNodes().ToList(), codeNode);
                codeNode.ChildNodes = childNodes;
                list.Add(codeNode);
            }

            return list;
        }

        private static readonly List<SyntaxKind> ParentsWithUniqueBlock = new List<SyntaxKind>()
        {
            SyntaxKind.ClassDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.DoStatement,
            SyntaxKind.ParenthesizedExpression,
            SyntaxKind.ParenthesizedLambdaExpression
        };

        private static IEnumerable<string> GetUniqueIdentifiers(SyntaxNode syntaxNode, CodeNode parent)
        {
            if (syntaxNode == null)
                return new List<string>();

            switch (syntaxNode)
            {
                case CompilationUnitSyntax compilationUnitSyntax:
                    return new [] { compilationUnitSyntax.Language }; // compilation unit is a singleton, so allow it to be treated as such
                case ClassDeclarationSyntax classDeclarationSyntax:
                    return new[] { classDeclarationSyntax.Identifier.ToString() };
                case StructDeclarationSyntax structDeclarationSyntax:
                    return new[] { structDeclarationSyntax.Identifier.ToString() };
                case EnumDeclarationSyntax enumDeclarationSyntax:
                    return new[] { enumDeclarationSyntax.Identifier.ToString() };
                case EnumMemberDeclarationSyntax enumMemberDeclarationSyntax:
                    return new[] { enumMemberDeclarationSyntax.Identifier.ToString() };
                case EventDeclarationSyntax eventDeclarationSyntax:
                    return new[] { eventDeclarationSyntax.Identifier.ToString() };
                case FieldDeclarationSyntax fieldDeclarationSyntax:
                    return fieldDeclarationSyntax.Declaration.Variables.Select(v => v.Identifier.ToString());
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return new[] { methodDeclarationSyntax.Identifier.ToString() };
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    return new[] { namespaceDeclarationSyntax.Name.ToString() };
                case OperatorDeclarationSyntax operatorDeclarationSyntax:
                    return new[] { operatorDeclarationSyntax.OperatorToken.ToString() };
                case PropertyDeclarationSyntax propertyDeclarationSyntax:
                    return new[] { propertyDeclarationSyntax.Identifier.ToString() };
                case EventFieldDeclarationSyntax eventFieldDeclarationSyntax:
                    return eventFieldDeclarationSyntax.Declaration.Variables.Select(v => v.Identifier.ToString());
                case ParameterSyntax parameterSyntax:
                    return new[] { parameterSyntax.Identifier.ToString()};
                case LocalDeclarationStatementSyntax localDeclarationStatementSyntax:
                    return localDeclarationStatementSyntax.Declaration.Variables.Select(v => v.Identifier.ToString());
                case VariableDeclarationSyntax variableDeclarationSyntax:
                    return variableDeclarationSyntax.Variables.Select(v => v.Identifier.ToString());
                case VariableDeclaratorSyntax variableDeclaratorSyntax:
                    return new[] { variableDeclaratorSyntax.Identifier.ToString()};
                case SwitchStatementSyntax switchStatementSyntax:
                    return new[] { switchStatementSyntax.Expression.ToString()};
                case SwitchSectionSyntax switchSectionSyntax:
                    return switchSectionSyntax.Labels.Select(l => l.ToString());
                case BlockSyntax blockSyntax: 
                    if (ParentsWithUniqueBlock.Contains(parent.Kind))
                        return new[] { "Body" }; // make block unique so the diff thinks it's a modify instead of an add+remove
                    break;
                case ParameterListSyntax parameterListSyntax:
                    if (ParentsWithUniqueBlock.Contains(parent.Kind))
                        return new[] { "Parameters" }; // make block unique so the diff thinks it's a modify instead of an add+remove
                    break;
            }
            return new List<string>();
        }
    }
}
