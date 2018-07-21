using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.CSharp
{
    public class CodeNode
    {
        public CodeNode(SyntaxKind kind, string source)
        {
            Kind = kind;
            Source = source;
            ChildNodes = new List<CodeNode>();
        }

        public SyntaxKind Kind { get; }
        public string Source { get; }
        public List<CodeNode> ChildNodes { get; set; }
    }
}
