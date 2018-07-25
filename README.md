# itinerary

## What Itinerary does for you
Itinerary is a tool that uses Roslyn to help you make C# courses by extracting human readable migration steps from the version history of a source repository. "Add method 'Add() ...' to class x", "Add 'using System.IO' here", you get the idea... You can then use those migration steps as a skeleton for your course, or even generate a code timeline browser for your website from it.

## How Itinerary works
As an example, here's how I work:

I use it with git (but it works on folders so it's VCS agnostic):

I separate my course into functional increments/steps (like in TDD, actually). I write the code in that order and commit to git as normal. For each functional increment i add another branch. So, you end up with a sequence of branches pointing to the different steps you wish to teach in the course.

Using branches is important so you can always add corrections to an earlier step later on. When all steps are course-ready, you extract each of them to a separate folder and let Itinerary detect migrations between them.

Itinerary is a commandline tool that accepts either a list of folders to compare (each folder is a version of the sourcecode), or a single rootfolder (of wich all subfolders are supposed to be versions of your project).

For each folder pair, Itinerary walks the directory-tree and finds added and removed files. It also jumps into files to find changes there. The changes are currently stored in an HTML report for debugging purposes, but obviously that will change. The point is that changes are detected.

## Itinerary is just a generic tree comparer

Most data comes in trees: folders, C# code, project files, solution files, ... all hierarchical data. Itinerary is therefore just a generic tree walker: it starts by making a node for the root folder, and then starts expanding nodes into subnodes. The Expander is chosen based on the node type: DirectoryNodes get a DirectoryExpander, CSharpFileNodes get a CSharpExpander, etc. This way, we get just one big tree containing everything we find. We can then use the tree to compare it to a similar tree (another version) and find out what has changed.
