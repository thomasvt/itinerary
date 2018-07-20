using System.Collections.Generic;
using Itinerary.DiffTree;

namespace Itinerary.DiffTreeBuidling
{
    public interface IFileContentComparer
    {
        string FileExtension { get; }
        List<DiffTreeNode> Parse(string leftFilename, string rightFilename);
    }
}
