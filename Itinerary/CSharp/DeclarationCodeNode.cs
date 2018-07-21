using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.CSharp
{
    public class DeclarationCodeNode
    : CodeNode
    {
        public DeclarationCodeNode(SyntaxKind kind, List<string> identifiers, string source) : base($"{kind} {string.Join(", ", identifiers)}", kind, source)
        {
            Identifiers = identifiers;
        }

        public List<string> Identifiers { get; }
    }
}