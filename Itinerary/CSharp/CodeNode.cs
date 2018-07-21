using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.CSharp
{
    public class CodeNode
    {
        public CodeNode(string label, SyntaxKind kind, string source)
        {
            Label = label;
            Kind = kind;
            Source = source;
            ChildNodes = new List<CodeNode>();
        }

        public override string ToString()
        {
            return Label;
        }

        public SyntaxKind Kind { get; }
        public string Source { get; }
        public List<CodeNode> ChildNodes { get; internal set; }
        public string Label { get; }
    }
}
