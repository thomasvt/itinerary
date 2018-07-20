using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Itinerary.DiffTree;

namespace Itinerary.Report
{
    public class ReportBuilder
    {
        public static void BuildReport(DiffTree.DiffTree tree, string filename)
        {
            var doc = new HtmlDocument();
            var htmlNode = doc.DocumentNode.AppendChild(HtmlNode.CreateNode("<html>"));
            var headNode = htmlNode.AppendChild(HtmlNode.CreateNode("<head>"));
            headNode.AppendChild(HtmlNode.CreateNode(
                "<link rel=\"stylesheet\" href=\"https://use.fontawesome.com/releases/v5.1.1/css/all.css\" integrity=\"sha384-O8whS3fhG2OnA5Kas0Y9l3cfpmYjapjI0E4theH4iuMD+pLhbf6JI0jIMfYcK3yZ\" crossorigin=\"anonymous\">"));
            var bodyNode =
                htmlNode.AppendChild(HtmlNode.CreateNode("<body style=\"font-family: Arial, Helvetica, sans-serif;\">"));
            AddNodesToHtmlDoc(tree.Nodes, bodyNode);
            doc.Save(File.Open(filename, FileMode.Create));
        }

        private static void AddNodesToHtmlDoc(List<DiffNode> nodes, HtmlNode parentNode)
        {
            var ulNode = parentNode.AppendChild(HtmlNode.CreateNode("<ul>"));
            foreach (var node in nodes)
            {
                var fileSign = node.ObjectType == ObjectType.Directory ? "<i class=\"fas fa-folder-open\" style=\"color: #FCE181;\"></i>" : "<i class=\"fas fa-file\" style=\"color: #909292;\"></i>";
                var liNode = ulNode.AppendChild(HtmlNode.CreateNode($"<li>{GetCompareSign(node.ChangeType)} {fileSign} {node.Name}</li>"));
                if (node.ChildNodes.Any())
                {
                    AddNodesToHtmlDoc(node.ChildNodes, liNode);
                }
            }
        }

        private static string GetCompareSign(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Unmodified:
                    return "=";
                case ChangeType.Modified:
                    return "!=";
                case ChangeType.Added:
                    return "+";
                case ChangeType.Removed:
                    return "-";
            }
            return "?";
        }
    }
}
