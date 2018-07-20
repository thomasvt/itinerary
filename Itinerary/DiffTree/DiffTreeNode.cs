using System.Collections.Generic;

namespace Itinerary.DiffTree
{
    public class DiffTreeNode
    {
        public DiffTreeNode(string name, ObjectType objectType, ChangeType changeType)
        {
            Name = name;
            ObjectType = objectType;
            ChangeType = changeType;
            ChildNodes = new List<DiffTreeNode>();
        }

        public string Name { get; }
        public ObjectType ObjectType { get; }
        public IReadOnlyList<DiffTreeNode> ChildNodes { get; internal set; }
        public ChangeType ChangeType { get; }
    }
}
