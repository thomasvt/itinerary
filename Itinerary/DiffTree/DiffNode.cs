using System.Collections.Generic;

namespace Itinerary.DiffTree
{
    public class DiffNode
    {
        public DiffNode(string name, ObjectType objectType, ChangeType changeType)
        {
            Name = name;
            ObjectType = objectType;
            ChangeType = changeType;
            ChildNodes = new List<DiffNode>();
        }

        public string Name { get; }
        public ObjectType ObjectType { get; }
        public IReadOnlyList<DiffNode> ChildNodes { get; internal set; }
        public ChangeType ChangeType { get; }
    }
}
