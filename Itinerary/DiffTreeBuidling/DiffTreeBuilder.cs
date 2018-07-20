using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.DiffTree;

namespace Itinerary.DiffTreeBuidling
{
    internal class DiffTreeBuilder
    {
        private readonly Dictionary<string, IFileContentComparer> _fileContentComparers;

        public DiffTreeBuilder()
        {
            _fileContentComparers = new Dictionary<string, IFileContentComparer>();
            IgnoreFolders = new List<string>();
        }

        public DiffTree.DiffTree BuildDiffTree(string leftFolder, string rightFolder)
        {
            var nodes = GetNodesForFolderPair(leftFolder, rightFolder);
            return new DiffTree.DiffTree(nodes);
        }

        private List<DiffTreeNode> GetNodesForFolderPair(string leftFolder, string rightFolder)
        {
            var nodes = new List<DiffTreeNode>();
            AddFolderNodes(leftFolder, rightFolder, nodes);
            AddFileNodes(leftFolder, rightFolder, nodes);
            return nodes;
        }

        public void RegisterFileContentComparer(IFileContentComparer fileContentComparer)
        {
            _fileContentComparers.Add(fileContentComparer.FileExtension, fileContentComparer);
        }

        private void AddFolderNodes(string leftFolder, string rightFolder, List<DiffTreeNode> nodes)
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
                    var node = new DiffTreeNode(remainedFolder, ObjectType.Directory, changeType)
                    {
                        ChildNodes = childNodes.AsReadOnly()
                    };
                    nodes.Add(node);
                },
                removedFolder =>
                {
                    var node = new DiffTreeNode(removedFolder, ObjectType.Directory, ChangeType.Removed);
                    nodes.Add(node);
                    node.ChildNodes = GetNodesForFolderPair(Path.Combine(leftFolder, removedFolder), null);
                },
                addedFolder =>
                {
                    var node = new DiffTreeNode(addedFolder, ObjectType.Directory, ChangeType.Added);
                    nodes.Add(node);
                    node.ChildNodes = GetNodesForFolderPair(null, Path.Combine(rightFolder, addedFolder));
                });
        }

        private void AddFileNodes(string leftFolder, string rightFolder, List<DiffTreeNode> nodes)
        {
            var leftFileList = leftFolder != null ? GetFiles(leftFolder) : new List<string>();
            var rightFileList = rightFolder != null ? GetFiles(rightFolder) : new List<string>();
            TwoWayCompare(leftFileList, rightFileList,
                remainedFile =>
                {
                    var leftFullFileName = Path.Combine(leftFolder, remainedFile);
                    var rightFullFilename = Path.Combine(rightFolder, remainedFile);
                    var areEqual = FileUtils.FileContentsAreEqual(leftFullFileName, rightFullFilename);
                    var node = new DiffTreeNode(remainedFile, ObjectType.File, areEqual ? ChangeType.Unmodified : ChangeType.Modified);
                    nodes.Add(node);
                    node.ChildNodes = GetNodesForFileContent(leftFullFileName, rightFullFilename);
                },
                removedFile => nodes.Add(new DiffTreeNode(removedFile, ObjectType.File, ChangeType.Removed)),
                addedFile => nodes.Add(new DiffTreeNode(addedFile, ObjectType.File, ChangeType.Added))
            );
        }

        private IReadOnlyList<DiffTreeNode> GetNodesForFileContent(string leftFileName, string rightFilename)
        {
            var extension = Path.GetExtension(leftFileName);
            if (_fileContentComparers.ContainsKey(extension))
            {
                var fileContentComparer = _fileContentComparers[extension];
                try
                {
                    return fileContentComparer.Parse(leftFileName, rightFilename).AsReadOnly();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{fileContentComparer.GetType().Name} returned an error while comparing:");
                    Console.WriteLine($" A: {leftFileName}");
                    Console.WriteLine($" B: {rightFilename}");
                    Console.WriteLine($" {e.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    return new List<DiffTreeNode>() { new DiffTreeNode("#contentError#", ObjectType.Message, ChangeType.Unmodified)};
                }
            }
            return new List<DiffTreeNode>();
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

        public List<string> IgnoreFolders { get; set; }
    }
}
