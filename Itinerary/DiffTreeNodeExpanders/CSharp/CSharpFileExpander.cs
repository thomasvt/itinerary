using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.Comparing;
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
            node.ChildNodes = BuildCSharpTree(leftTree, rightTree).ToList();
        }

        private static List<CodeNode> GetSemanticTree(string leftFilename)
        {
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(leftFilename)).GetRoot();
            return TreeExtracter.ExtractTreeOfInterest(new [] { root });
        }
        
        internal static IEnumerable<DiffTreeNode> BuildCSharpTree(List<CodeNode> leftList, List<CodeNode> rightList)
        {
            // probably can be done cleaner
            // left and right list are CodeNodes that must be compared.
            // if a node is an IdentifiableCodeNode, it has a unique identifier:
            // -> use that to find it in the other list, even though its code content may be different
            // if its not an IdentifiableCodeNode, comparison is done by code content and will only match if the code is identical

            leftList = leftList ?? new List<CodeNode>(); // deliberate null pattern, we want to support non existing trees (=empty) too.
            rightList = rightList ?? new List<CodeNode>();

            var list = new List<DiffTreeNode>();
            var changes = UnorderedListComparer.Compare(leftList, rightList, (leftNode, rightNode) =>
            {
                if (leftNode is IdentifiableCodeNode leftDcn && rightNode is IdentifiableCodeNode rightDcn)
                    return leftDcn.Equals(rightDcn);
                return string.Equals(leftNode?.Source, rightNode?.Source);
            }).ToList();

            foreach (var change in changes)
            {
                var childNodes = new List<DiffTreeNode>();
                if (change.ChangeType == ChangeType.Unmodified)
                {
                    childNodes = BuildCSharpTree(change.LeftItem.ChildNodes, change.RightItem.ChildNodes).ToList();
                }

                var label = childNodes.Any()
                    ? change.Item.Label
                    : $"{change.Item.Label} {change.Item.Source}";

                var item = new DiffTreeNode(label, "", "", ObjectType.CodeConstruct)
                {
                    ChangeType = change.ChangeType,
                    ChildNodes = childNodes
                };

                if (item.ChangeType == ChangeType.Unmodified && item.ChildNodes.Any(cn => cn.ChangeType != ChangeType.Unmodified))
                {
                    item.ChangeType = ChangeType.Modified;
                }
                list.Add(item);
            }
            return list;
        }

        public bool IsLeafExpander => true;
    }
}
