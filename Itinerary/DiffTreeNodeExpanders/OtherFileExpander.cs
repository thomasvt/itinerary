using System;
using Itinerary.Comparing;
using Itinerary.DiffTree;
using Itinerary.DiffTreeBuilding;

namespace Itinerary.DiffTreeNodeExpanders
{
    public class OtherFileExpander
    : IDiffTreeNodeExpander
    {
        public bool CanExpand(DiffTreeNode node)
        {
            return node.NodeType == NodeType.File;
        }

        public bool IsLeafExpander => true;

        public void Expand(DiffTreeNode node)
        {
            node.ChangeType = FileUtils.FileContentsAreEqual(node.LeftFullPath, node.RightFullPath) ? ChangeType.Unmodified : ChangeType.Modified;
        }
    }
}
