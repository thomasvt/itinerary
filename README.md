# itinerary

## What Itinerary does for you
Itinerary is a tool that uses Roslyn to help you make C# courses by extracting human readable migration steps from the version history of a source repository. "Add method 'Add() ...' to class x", "Add 'using System.IO' here", you get the idea... You can then use those migration steps as a skeleton for your course, or even generate a code timeline browser for your website from it.

Of course, Itinerary can't do magic: it's up to you to have clean increments per version of the sources. But you can make that a bit easier by having eg. a VCS branch per course step (version) which you can tweak until good enough.

## How Itinerary works
As example, here's how I work:

I use it with git (but it works on folders so it's VCS agnostic):

I separate my course into functional increments/steps (like in TDD, actually). I write the code in that order and commit to git as normal. For each functional increment i add another branch. So, you end up with a sequence of branches pointing to the different steps you wish to teach in the course.

Using branches is important so you can always add corrections to an earlier step later on. When all steps are course-ready, you extract each of them to a separate folder and let Itinerary detect migrations between them.
