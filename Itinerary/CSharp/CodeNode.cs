using System.Collections.Generic;

namespace Itinerary.CSharp
{
    public class CodeNode
    {
        public CodeNode(string label)
        {
            Label = label;
            ChildNodes = new List<CodeNode>();
        }

        public string Label { get; }
        public List<CodeNode> ChildNodes { get; set; }
    }
}
