using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Itinerary
{
    public static class CmdLnOptions
    {
        public static bool Parse(string[] args)
        {
            try
            {
                ParseOptions(args);
                ParsePaths(args);
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
            if (options.Contains("-a"))
            {
                options.Remove("-a");
                ChangesOnly = true;
            }

            if (options.Any())
            {
                throw new CmdLnException("Unknown command line option(s): " + string.Join(" ", options));
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
            Console.WriteLine("-a       List all objects (instead of only the changed ones)");
        }

        public static bool ChangesOnly { get; private set; }
        public static List<string> Paths { get; private set; }
    }
}
