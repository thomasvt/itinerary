using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var leftTree = GetSemanticTree(node.LeftFullPath);
            var rightTree = GetSemanticTree(node.RightFullPath);
            node.ChildNodes = BuildCSharpTree(leftTree, rightTree).ToList();
        }

        private static List<CodeNode> GetSemanticTree(string filename)
        {
            if (filename == null)
                return new List<CodeNode>(); // a file may not exist on one side if it's an Add or Remove
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(filename)).GetRoot();
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
                return string.Equals(RemoveWhiteSpace(leftNode?.Source), RemoveWhiteSpace(rightNode?.Source));
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
                    : $"{change.Item.Label}{Environment.NewLine}{change.Item.Source}";

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

        private static string RemoveWhiteSpace(string source)
        {
            if (source == null)
                return null;
            var sb = new StringBuilder(source.Length);
            var firstWhitespace = true;
            for (var i = 0; i < source.Length; i++)
            {
                var ch = source[i];
                if (ch == ' ' || ch == 13 || ch == 10 || ch == 9)
                {
                    if (firstWhitespace)
                        sb.Append(' ');
                    firstWhitespace = false;
                }
                else
                {
                    firstWhitespace = true;
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        public bool IsLeafExpander => true;
    }
}
