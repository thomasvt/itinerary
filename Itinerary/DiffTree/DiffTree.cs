using System.Collections.Generic;

namespace Itinerary.DiffTree
{
    public class DiffTree
    {
        internal DiffTree(List<DiffNode> nodes)
        {
            Nodes = nodes;
        }

        public List<DiffNode> Nodes { get; }
    }
}
