using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Itinerary.Comparing;
using Itinerary.DiffTree;

namespace Itinerary.Report
{
    public class ReportBuilder
    {
        public void BuildReport(DiffTreeNode root, string filename)
        {
            var doc = new HtmlDocument();
            var htmlNode = doc.DocumentNode.AppendChild(HtmlNode.CreateNode("<html>"));
            var headNode = htmlNode.AppendChild(HtmlNode.CreateNode("<head>"));
            headNode.AppendChild(HtmlNode.CreateNode(
                "<link rel=\"stylesheet\" href=\"https://use.fontawesome.com/releases/v5.1.1/css/all.css\" integrity=\"sha384-O8whS3fhG2OnA5Kas0Y9l3cfpmYjapjI0E4theH4iuMD+pLhbf6JI0jIMfYcK3yZ\" crossorigin=\"anonymous\">"));
            var bodyNode =
                htmlNode.AppendChild(HtmlNode.CreateNode("<body style=\"font-family: Courier New,Courier,Lucida Sans Typewriter,Lucida Typewriter,monospace;\">"));
            AddNodesToHtmlDoc(new [] { root }, bodyNode);
            doc.Save(File.Open(filename, FileMode.Create));
        }

        private void AddNodesToHtmlDoc(IEnumerable<DiffTreeNode> nodes, HtmlNode parentNode)
        {
            var ulNode = parentNode.AppendChild(HtmlNode.CreateNode("<ul style=\"list-style-type: none; -webkit-padding-start: 20px;\">"));
            if (!ShowAll)
                nodes = nodes.Where(n => n.ChangeType != ChangeType.Unmodified);
            foreach (var node in nodes)
            {
                var objectTypeIcon = GetObjectTypeIcon(node);
                var changeTypeIcon = GetChangeTypeIcon(node.ChangeType);
                var name = HtmlDocument.HtmlEncode(node.Name).Replace(" ", "&nbsp;").Replace(Environment.NewLine, "<br/>"); // spaces and newlines are not encoded :S
                var liNode = ulNode.AppendChild(HtmlNode.CreateNode($"<li>{changeTypeIcon}&nbsp; &nbsp;{objectTypeIcon} {name}</li>"));
                if (node.ChildNodes.Any())
                {
                    AddNodesToHtmlDoc(node.ChildNodes, liNode);
                }
            }
        }

        private static string GetObjectTypeIcon(DiffTreeNode node)
        {
            switch (node.NodeType)
            {
                case NodeType.File:
                    return "<i class=\"fas fa-file\" style=\"color: #909292;\"></i>";
                case NodeType.Directory:
                    return "<i class=\"fas fa-folder-open\" style=\"color: #D8AC6A;\"></i>";
                case NodeType.Message:
                    return "<i class=\"fas fa-comment\" style=\"color: #909292;\"></i>";
                case NodeType.Namespace:
                    return "<i class=\"fas fa-code\" style=\"color: #909292;\"></i>";
                case NodeType.Class:
                    return "<i class=\"fas fa-project-diagram\" style=\"color: #F8D96E;\"></i>";
                case NodeType.Method:
                    return "<i class=\"fas fa-cube\" style=\"color: #6C4A86;\"></i>";
                case NodeType.Property:
                    return "<i class=\"fas fa-wrench\" style=\"color: #909292;\"></i>";
                case NodeType.Field:
                    return "<i class=\"fas fa-inbox\" style=\"color: #4478A6;\"></i>";
                case NodeType.CSharp:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetChangeTypeIcon(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Unmodified:
                    return "<i class=\"fas fa-equals\" style=\"font-size: 10;\" title=\"unmodified\"></i>";
                case ChangeType.Modified:
                    return "<i class=\"fas fa-not-equal\" style=\"font-size: 10;color: #F9A825;\" title=\"modified\"></i>";
                case ChangeType.Added:
                    return "<i class=\"fas fa-asterisk\" style=\"font-size: 10;color: #558B2F;\" title=\"added\"></i>";
                case ChangeType.Removed:
                    return "<i class=\"fas fa-trash\" style=\"font-size: 10;color: #c62828;\" title=\"removed\"></i>";
            }
            return "?";
        }

        public bool ShowAll { get; set; }
    }
}
