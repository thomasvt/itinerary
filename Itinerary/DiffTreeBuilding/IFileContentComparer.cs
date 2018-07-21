using System.Collections.Generic;
using Itinerary.DiffTree;

namespace Itinerary.DiffTreeBuilding
{
    public interface IFileContentComparer
    {
        string FileExtension { get; }
        List<DiffTreeNode> Parse(string leftFilename, string rightFilename);
    }
}
