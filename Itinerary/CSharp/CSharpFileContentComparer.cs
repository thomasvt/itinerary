using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.Comparing;
using Itinerary.DiffTree;
using Itinerary.DiffTreeBuidling;
using Microsoft.CodeAnalysis.CSharp;

namespace Itinerary.CSharp
{
    public class CSharpFileContentComparer
    : IFileContentComparer
    {
        public string FileExtension => ".cs";

        public List<DiffTreeNode> Parse(string leftFilename, string rightFilename)
        {
            var leftTree = GetSemanticTree(leftFilename);
            var rightTree = GetSemanticTree(rightFilename);
            return GetDiffTree(leftTree, rightTree).ToList();
        }

        private static List<CodeNode> GetSemanticTree(string leftFilename)
        {
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(leftFilename)).GetRoot();
            return TreeExtracter.ExtractTreeOfInterest(new [] { root });
        }

        private static DiffTreeNode GetDiffTreeNode(CodeNode codeNode)
        {
            return new DiffTreeNode($"{codeNode.Label} {codeNode.Source}", ObjectType.Other, ChangeType.Unmodified)
            {
                //ChildNodes = GetDiffTree(codeNode.ChildNodes).ToList()
            };
        }

        private static IEnumerable<DiffTreeNode> GetDiffTree(List<CodeNode> leftList, List<CodeNode> rightList)
        {
            // detect changes between two unordered lists
            var leftIndex = 0;
            var rightIndex = 0;
            return new List<DiffTreeNode>();
        }
    }
}
