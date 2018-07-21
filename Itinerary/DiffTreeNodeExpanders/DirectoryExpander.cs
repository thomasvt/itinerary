using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.Comparing;
using Itinerary.DiffTree;
using Itinerary.DiffTreeBuilding;

namespace Itinerary.DiffTreeNodeExpanders
{
    public class DirectoryExpander
    : IDiffTreeNodeExpander
    {
        public DirectoryExpander()
        {
            IgnoreFolders = new List<string>();
        }

        public bool CanExpand(DiffTreeNode node)
        {
            return node.NodeType == NodeType.Directory;
        }

        public void Expand(DiffTreeNode node)
        {
            node.ChildNodes = GetNodesForFolderPair(Path.Combine(node.LeftParent, node.Name ?? ""), Path.Combine(node.RightParent, node.Name ?? ""));
        }

        private List<DiffTreeNode> GetNodesForFolderPair(string leftFolder, string rightFolder)
        {
            var nodes = new List<DiffTreeNode>();
            AddFolderNodes(leftFolder, rightFolder, nodes);
            AddFileNodes(leftFolder, rightFolder, nodes);
            return nodes;
        }

        private void AddFolderNodes(string leftFolder, string rightFolder, List<DiffTreeNode> nodes)
        {
            var leftList = leftFolder != null ? GetSubFolders(leftFolder) : new List<string>();
            var rightList = rightFolder != null ? GetSubFolders(rightFolder) : new List<string>();
            var changes = OrderedListComparer.Compare(leftList, rightList, (left, right) => string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase));
            foreach (var change in changes)
            {
                var node = new DiffTreeNode(change.LeftItem ?? change.RightItem, leftFolder, rightFolder, NodeType.Directory)
                {
                    ChangeType = change.ChangeType
                };
                nodes.Add(node);
            }
        }

        private void AddFileNodes(string leftFolder, string rightFolder, List<DiffTreeNode> nodes)
        {
            var leftFileList = leftFolder != null ? GetFiles(leftFolder) : new List<string>();
            var rightFileList = rightFolder != null ? GetFiles(rightFolder) : new List<string>();
            var changes = OrderedListComparer.Compare(leftFileList, rightFileList, (left, right) => string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase));
            foreach (var change in changes)
            {
                var node = new DiffTreeNode(change.LeftItem ?? change.RightItem, leftFolder, rightFolder, NodeType.File)
                {
                    ChangeType = change.ChangeType
                };
                nodes.Add(node);
            }
        }

        private List<string> GetSubFolders(string rootFolder)
        {
            return Directory.GetDirectories(rootFolder)
                .Select(Path.GetFileName)
                .Where(d => !d.StartsWith(".") && !IgnoreFolders.Contains(d, StringComparer.InvariantCultureIgnoreCase))
                .OrderBy(d => d, StringComparer.InvariantCultureIgnoreCase)
                .ToList();
        }

        private static List<string> GetFiles(string leftFolder)
        {
            return Directory.GetFiles(leftFolder)
                .Where(f => !new FileInfo(f).Attributes.HasFlag(FileAttributes.Hidden))
                .Select(Path.GetFileName)
                .Where(d => !d.StartsWith("."))
                .OrderBy(n => n, StringComparer.InvariantCultureIgnoreCase)
                .ToList();
        }

        public bool IsLeafExpander => false;
        public List<string> IgnoreFolders { get; set; }
    }
}
