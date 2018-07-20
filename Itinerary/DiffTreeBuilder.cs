using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.DiffTree;

namespace Itinerary
{
    internal class DiffTreeBuilder
    {
        public DiffTreeBuilder()
        {
            IgnoreFolders = new List<string>();
        }

        public DiffTree.DiffTree BuildDiffTree(string leftFolder, string rightFolder)
        {
            var nodes = GetNodesForFolderPair(leftFolder, rightFolder);
            return new DiffTree.DiffTree(nodes);
        }

        private List<DiffNode> GetNodesForFolderPair(string leftFolder, string rightFolder)
        {
            var nodes = new List<DiffNode>();
            AddFolderNodes(leftFolder, rightFolder, nodes);
            AddFileNodes(leftFolder, rightFolder, nodes);
            return nodes;
        }

        private void AddFolderNodes(string leftFolder, string rightFolder, List<DiffNode> nodes)
        {
            var leftList = leftFolder != null ? GetSubFolders(leftFolder) : new List<string>();
            var rightList = rightFolder != null ? GetSubFolders(rightFolder) : new List<string>();
            TwoWayCompare(leftList, rightList,
                remainedFolder =>
                {
                    var childNodes = GetNodesForFolderPair(Path.Combine(leftFolder, remainedFolder), Path.Combine(rightFolder, remainedFolder));
                    var changeType = childNodes.Any(n => n.ChangeType != ChangeType.Unmodified)
                        ? ChangeType.Modified
                        : ChangeType.Unmodified;
                    var node = new DiffNode(remainedFolder, ObjectType.Directory, changeType)
                    {
                        ChildNodes = childNodes.AsReadOnly()
                    };
                    nodes.Add(node);
                },
                removedFolder =>
                {
                    var node = new DiffNode(removedFolder, ObjectType.Directory, ChangeType.Removed);
                    nodes.Add(node);
                    node.ChildNodes = GetNodesForFolderPair(Path.Combine(leftFolder, removedFolder), null);
                },
                addedFolder =>
                {
                    var node = new DiffNode(addedFolder, ObjectType.Directory, ChangeType.Added);
                    nodes.Add(node);
                    node.ChildNodes = GetNodesForFolderPair(null, Path.Combine(rightFolder, addedFolder));
                });
        }

        private void AddFileNodes(string leftFolder, string rightFolder, List<DiffNode> nodes)
        {
            var leftFileList = leftFolder != null ? GetFiles(leftFolder) : new List<string>();
            var rightFileList = rightFolder != null ? GetFiles(rightFolder) : new List<string>();
            TwoWayCompare(leftFileList, rightFileList,
                remainedFile =>
                {
                    var areEqual = FileUtils.FileContentsAreEqual(Path.Combine(leftFolder, remainedFile), Path.Combine(rightFolder, remainedFile));
                    nodes.Add(new DiffNode(remainedFile, ObjectType.File, areEqual ? ChangeType.Unmodified : ChangeType.Modified));
                },
                removedFile => nodes.Add(new DiffNode(removedFile, ObjectType.File, ChangeType.Removed)),
                addedFile => nodes.Add(new DiffNode(addedFile, ObjectType.File, ChangeType.Added))
            );
        }

        /// <summary>
        /// Finds the operations that occurred to transform leftList into rightList: remove, add and remain.
        /// The corresponding callbacks are called as these operations are encounteres from top to bottom of the lists.
        /// </summary>
        private static void TwoWayCompare(List<string> leftList, List<string> rightList, Action<string> remainAction, Action<string> removeAction, Action<string> addAction)
        {
            var leftIndex = 0;
            var rightIndex = 0;
            while (leftIndex < leftList.Count || rightIndex < rightList.Count)
            {
                var left = leftIndex >= leftList.Count ? null : leftList[leftIndex];
                var right = rightIndex >= rightList.Count ? null : rightList[rightIndex];
                var compareResult = String.Compare(left, right, StringComparison.InvariantCultureIgnoreCase);

                if (compareResult == 0)
                {
                    remainAction(left);
                    leftIndex++;
                    rightIndex++;
                }
                else if (compareResult == -1 && left != null || right == null)
                {
                    removeAction(left);
                    leftIndex++;
                }
                else
                {
                    addAction(right);
                    rightIndex++;
                }
            }
        }

        private List<string> GetSubFolders(string rootFolder)
        {
            return Directory.GetDirectories(rootFolder)
                .Select(Path.GetFileName)
                .Where(d => !d.StartsWith(".") && !IgnoreFolders.Contains(d))
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

        public List<string> IgnoreFolders { get; set; }
    }
}
