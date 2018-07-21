using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.DiffTreeNodeExpanders.CSharp
{
    /// <summary>
    /// A node representing Identifiable code (= a code construct with a unique name within its scope) eg methods, fields, properties, classes
    /// </summary>
    public class IdentifiableCodeNode
    : CodeNode
    {
        public IdentifiableCodeNode(SyntaxKind kind, List<string> identifiers, string source) : base($"{kind} {string.Join(", ", identifiers)}", kind, source)
        {
            Identifiers = identifiers;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IdentifiableCodeNode))
                return false;
            var other = (IdentifiableCodeNode) obj;
            return Kind == other.Kind && Identifiers.TrueForAll(s => other.Identifiers.Contains(s));
        }

        public List<string> Identifiers { get; }
    }
}