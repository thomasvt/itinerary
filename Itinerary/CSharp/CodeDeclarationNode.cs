using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.CSharp
{
    public class CodeDeclarationNode
    : CodeNode
    {
        public CodeDeclarationNode(SyntaxKind kind, List<string> identifiers, string source) : base(kind, source)
        {
            Identifiers = identifiers;
        }

        public List<string> Identifiers { get; }
    }
}