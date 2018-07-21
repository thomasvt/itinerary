using Itinerary.DiffTree;

namespace Itinerary.DiffTreeBuilding
{
    public interface IDiffTreeNodeExpander
    {
        bool CanExpand(DiffTreeNode node);

        void Expand(DiffTreeNode node);
    }
}
