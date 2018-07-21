using System.Collections.Generic;
using Itinerary.Comparing;

namespace Itinerary.DiffTree
{
    public class DiffTreeNode
    {
        public DiffTreeNode(string name, string leftParent, string rightParent, ObjectType objectType)
        {
            Name = name;
            LeftParent = leftParent;
            RightParent = rightParent;
            ObjectType = objectType;
            ChildNodes = new List<DiffTreeNode>();
        }

        public string Name { get; }
        public string LeftParent { get; }
        public string RightParent { get; }
        public ObjectType ObjectType { get; }
        public IReadOnlyList<DiffTreeNode> ChildNodes { get; internal set; }
        public ChangeType ChangeType { get; internal set; }
    }
}
