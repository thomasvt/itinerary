using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Itinerary.DiffTree;
using Itinerary.DiffTreeBuilding;
using Itinerary.DiffTreeNodeExpanders;
using Itinerary.DiffTreeNodeExpanders.CSharp;
using Itinerary.Report;

namespace Itinerary
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                ShowHeader();
                if (CmdLnOptions.Parse(args))
                {
                    var folderChain = GetFolderChain();
                    BuildReportForEachFolderPairIn(folderChain);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unexpected error:");
                Console.WriteLine(e.Message);
            }

            if (!Debugger.IsAttached) return;

            Console.WriteLine();
            Console.WriteLine("Press <enter> to close");
            Console.Read();
        }

        private static void ShowHeader()
        {
            Console.WriteLine($"Itinerary {Assembly.GetEntryAssembly().GetName().Version}");
        }

        private static IEnumerable<string> GetFolderChain()
        {
            return CmdLnOptions.Paths.Count == 1 
                ? Directory.GetDirectories(CmdLnOptions.Paths.Single()).OrderBy(n => n).ToList() 
                : CmdLnOptions.Paths;
        }

        private static void BuildReportForEachFolderPairIn(IEnumerable<string> folderChain)
        {
            string previousFolder = null;
            foreach (var folder in folderChain)
            {
                if (previousFolder != null)
                {
                    BuildReportForFolderPair(previousFolder, folder);
                }
                previousFolder = folder;
            }
        }

        private static void BuildReportForFolderPair(string leftFolder, string rightFolder)
        {
            var diffTreeBuilder = CreateDiffTreeBuilder();
            var root = new DiffTreeNode(".", leftFolder, rightFolder, NodeType.Directory);
            diffTreeBuilder.BuildTree(root);
            var filename = rightFolder + ".html";
            var reportBuilder = new ReportBuilder()
            {
                ShowAll = CmdLnOptions.ShowAll
            };
            reportBuilder.BuildReport(root, filename);
            Console.WriteLine("Created " + filename);
        }

        private static DiffTreeBuilder CreateDiffTreeBuilder()
        {
            var diffTreeBuilder = new DiffTreeBuilder();
            diffTreeBuilder.RegisterNodeExpander(new DirectoryExpander { IgnoreFolders = CmdLnOptions.IgnoreFolders });
            diffTreeBuilder.RegisterNodeExpander(new CSharpFileExpander());
            diffTreeBuilder.RegisterNodeExpander(new OtherFileExpander());
            return diffTreeBuilder;
        }
    }
}