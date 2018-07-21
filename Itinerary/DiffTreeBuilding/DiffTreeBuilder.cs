using System;
using System.Collections.Generic;
using System.Linq;
using Itinerary.DiffTree;

namespace Itinerary.DiffTreeBuilding
{
    internal class DiffTreeBuilder
    {
        private readonly List<IDiffTreeNodeExpander> _expanders;

        public DiffTreeBuilder()
        {
            _expanders = new List<IDiffTreeNodeExpander>();
        }

        /// <summary>
        /// Builds the tree by recursibely expanding your rootnode into childnodes using expanders on each new node.
        /// The first matching expander is executed per node.
        /// </summary>
        public void BuildTree(DiffTreeNode rootNode)
        {
            ExpandNodeAndChildren(rootNode);
        }

        private void ExpandNodeAndChildren(DiffTreeNode rootNode)
        {
            ExpandNode(rootNode);
            foreach (var childNode in rootNode.ChildNodes)
            {
                ExpandNodeAndChildren(childNode);
            }
        }

        private void ExpandNode(DiffTreeNode node)
        {
            var expander = _expanders.FirstOrDefault(e => e.CanExpand(node));
            try
            {
                expander?.Expand(node);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{expander.GetType().Name} returned an error while expanding:");
                Console.WriteLine($" Item:        {node.Name}");
                Console.WriteLine($" LeftParent:  {node.LeftParent}");
                Console.WriteLine($" RightParent: {node.RightParent}");
                Console.WriteLine($" {e.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                node.ChildNodes = new List<DiffTreeNode> {new DiffTreeNode($"#err#{expander.GetType().Name}#{e.Message}#", null, null, ObjectType.Message)};
            }
        }

        public void RegisterNodeExpander(IDiffTreeNodeExpander nodeExpander)
        {
            _expanders.Add(nodeExpander);
        }
    }
}
