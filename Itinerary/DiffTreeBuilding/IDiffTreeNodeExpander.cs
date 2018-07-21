using Itinerary.DiffTree;

namespace Itinerary.DiffTreeBuilding
{
    public interface IDiffTreeNodeExpander
    {
        bool CanExpand(DiffTreeNode node);
        /// <summary>
        /// Are the nodes created by this expander leaves? (=the recursive expanding can stop here)
        /// </summary>
        bool IsLeafExpander { get; }
        void Expand(DiffTreeNode node);
    }
}
