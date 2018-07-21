using System.Collections.Generic;
using System.IO;
using Itinerary.DiffTree;
using Itinerary.DiffTreeBuilding;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.DiffTreeNodeExpanders.CSharp
{
    public class CSharpFileExpander
    : IDiffTreeNodeExpander
    {
        private const string FileExtension = ".cs";
        
        public bool CanExpand(DiffTreeNode node)
        {
            return node.ObjectType == ObjectType.File && Path.GetExtension(node.Name) == FileExtension;
        }

        public void Expand(DiffTreeNode node)
        {
            var leftTree = GetSemanticTree(Path.Combine(node.LeftParent, node.Name));
            var rightTree = GetSemanticTree(Path.Combine(node.RightParent, node.Name));
            node.ChildNodes = BuildCSharpTree(leftTree, rightTree);
        }

        private static List<CodeNode> GetSemanticTree(string leftFilename)
        {
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(leftFilename)).GetRoot();
            return TreeExtracter.ExtractTreeOfInterest(new [] { root });
        }
        
        private static List<DiffTreeNode> BuildCSharpTree(List<CodeNode> leftList, List<CodeNode> rightList)
        {
            // detect changes between two unordered lists
            var leftIndex = 0;
            var rightIndex = 0;
            return new List<DiffTreeNode>();
        }

        public bool IsLeafExpander => true;
    }
}
