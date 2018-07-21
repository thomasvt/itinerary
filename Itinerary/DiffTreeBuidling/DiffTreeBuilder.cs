using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinerary.Comparing;
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
            var changes = Comparer.CompareSortedLists(leftList, rightList, (left, right) => string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase));
            foreach (var change in changes)
            {
                DiffTreeNode node;
                switch (change.ChangeType)
                {
                    case ChangeType.Unmodified:
                        var childNodes = GetNodesForFolderPair(Path.Combine(leftFolder, change.Item), Path.Combine(rightFolder, change.Item));
                        var changeType = childNodes.Any(n => n.ChangeType != ChangeType.Unmodified)
                            ? ChangeType.Modified
                            : ChangeType.Unmodified;
                        node = new DiffTreeNode(change.Item, ObjectType.Directory, changeType)
                        {
                            ChildNodes = childNodes.AsReadOnly()
                        };
                        nodes.Add(node);
                        break;
                    case ChangeType.Removed:
                        node = new DiffTreeNode(change.Item, ObjectType.Directory, ChangeType.Removed);
                        nodes.Add(node);
                        node.ChildNodes = GetNodesForFolderPair(Path.Combine(leftFolder, change.Item), null);
                        break;
                    case ChangeType.Added:
                        node = new DiffTreeNode(change.Item, ObjectType.Directory, ChangeType.Added);
                        nodes.Add(node);
                        node.ChildNodes = GetNodesForFolderPair(null, Path.Combine(rightFolder, change.Item));
                        break;
                }
            }
        }

        private void AddFileNodes(string leftFolder, string rightFolder, List<DiffTreeNode> nodes)
        {
            var leftFileList = leftFolder != null ? GetFiles(leftFolder) : new List<string>();
            var rightFileList = rightFolder != null ? GetFiles(rightFolder) : new List<string>();
            var changes = Comparer.CompareSortedLists(leftFileList, rightFileList, (left, right) => string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase));
            foreach (var change in changes)
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Unmodified:
                        var leftFullFileName = Path.Combine(leftFolder, change.Item);
                        var rightFullFilename = Path.Combine(rightFolder, change.Item);
                        var areEqual = FileUtils.FileContentsAreEqual(leftFullFileName, rightFullFilename);
                        var node = new DiffTreeNode(change.Item, ObjectType.File,
                            areEqual ? ChangeType.Unmodified : ChangeType.Modified);
                        nodes.Add(node);
                        node.ChildNodes = GetNodesForFileContent(leftFullFileName, rightFullFilename);
                        break;
                    case ChangeType.Removed:
                        nodes.Add(new DiffTreeNode(change.Item, ObjectType.File, ChangeType.Removed));
                        break;
                    case ChangeType.Added:
                        nodes.Add(new DiffTreeNode(change.Item, ObjectType.File, ChangeType.Added));
                        break;
                }
            }
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
