using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Itinerary
{
    public static class CmdLnOptions
    {
        private static readonly string[] IgnoreFoldersDefault = { "bin", "obj", "packages", "properties" };

        static CmdLnOptions()
        {
            IgnoreFolders = new List<string>();
        }

        public static bool Parse(string[] args)
        {
            try
            {
                ParseOptions(args);
                ParsePaths(args);
                if (Help)
                {
                    ShowHelp();
                    return false;
                }
                return true;
            }
            catch (CmdLnException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CommandLine error:");
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                ShowHelp();
                return false;
            }
        }

        private static void ParseOptions(string[] args)
        {
            var options = args.Where(a => a.StartsWith("-")).ToList();
            ParseOption(options, "-a", o => ShowAll = true);
            ParseOption(options, "-h", o => Help = true);
            ParseOption(options, "-i=", 
                o => IgnoreFolders.AddRange(o.Substring(3).Split(',')),
                () => IgnoreFolders.AddRange(IgnoreFoldersDefault));

            if (options.Any())
            {
                throw new CmdLnException("Unknown command line option(s): " + string.Join(" ", options));
            }
        }

        /// <param name="matchAction">invoked EACH time the optionPrefix matches!</param>
        /// <param name="noMatchAction">invokes if no option matches the optionPrefix</param>
        private static void ParseOption(ICollection<string> options, string optionPrefix, Action<string> matchAction, Action noMatchAction = null)
        {
            var matchingOptions = options.Where(o => o.StartsWith(optionPrefix)).ToList();
            var hasMatch = false;
            foreach (var matchingOption in matchingOptions)
            {
                matchAction(matchingOption);
                options.Remove(matchingOption);
                hasMatch = true;
            }

            if (!hasMatch)
            {
                noMatchAction?.Invoke();
            }
        }

        private static void ParsePaths(string[] args)
        {
            Paths = args.Where(a => !a.StartsWith("-")).ToList();

            if (!Paths.Any())
                throw new CmdLnException("At least one path expected as a command line argument");

            foreach (var path in Paths)
            {
                if (!Directory.Exists(path))
                {
                    throw new CmdLnException($"Directory {path} does not exist.");
                }
            }
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Usages:");
            Console.WriteLine("ITINERARY [options] path");
            Console.WriteLine("   Generates a delta document for all subfolders of path.");
            Console.WriteLine();
            Console.WriteLine("ITINERARY [options] path1 path2 ...");
            Console.WriteLine("   Generates a delta document for each linkpair in the chain of paths.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-h           Shows this help text");
            Console.WriteLine("-a           List all objects (unmodifieds too)");
            Console.WriteLine("-i=a,b,...   Ignore list for folders");
            Console.WriteLine("             Default: -i=" + string.Join(",", IgnoreFoldersDefault));
        }

        public static bool Help { get; private set; }
        public static bool ShowAll { get; private set; }
        public static List<string> Paths { get; private set; }
        public static List<string> IgnoreFolders { get; }
    }
}
