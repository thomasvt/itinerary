using System.Collections.Generic;

namespace Itinerary.DiffTree
{
    public class DiffTree
    {
        internal DiffTree(List<DiffTreeNode> nodes)
        {
            Nodes = nodes;
        }

        public List<DiffTreeNode> Nodes { get; }
    }
}
