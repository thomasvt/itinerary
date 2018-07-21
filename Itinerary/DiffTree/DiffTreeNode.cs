using System;
using System.Collections.Generic;
using System.IO;
using Itinerary.Comparing;

namespace Itinerary.DiffTree
{
    public class DiffTreeNode
    {
        public DiffTreeNode(string name, string leftParent, string rightParent, NodeType nodeType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LeftParent = leftParent ?? throw new ArgumentNullException(nameof(leftParent));
            RightParent = rightParent ?? throw new ArgumentNullException(nameof(rightParent));
            NodeType = nodeType;
            ChildNodes = new List<DiffTreeNode>();
        }

        public string LeftFullPath => ChangeType == ChangeType.Added ? null : Path.Combine(LeftParent, Name);
        public string RightFullPath => ChangeType == ChangeType.Removed ? null :  Path.Combine(RightParent, Name);
        public string FullPath => LeftFullPath ?? RightFullPath;

        public string Name { get; }

        public string LeftParent { get; }
        public string RightParent { get; }

        public NodeType NodeType { get; }
        public IReadOnlyList<DiffTreeNode> ChildNodes { get; internal set; }
        public ChangeType ChangeType { get; internal set; }
    }
}
