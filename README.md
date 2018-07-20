# itinerary

## What is does for you
Itinerary is a tool that uses Roslyn to help you make C# courses from steps automatically extracted from the version history of a source repository. "Add method x here", "Insert statement x between y and z", etc. You can then use those migration steps as a skeleton for your course.

Of course, Itinerary can't do magic: writing quality courses still require work. It's up to you to have clean increments per version in your source repo, and to write decent explanations based on the generated migrations.

## How it works
I use it with git, but it works on folders, so it's VCS agnostic. Just get your different versions of the source in a bunch of separate folders and allow Itinerary to detect the changes.

As example, here's how I work:

I separate my source code into functional increments that will become the course's steps (very TDD, actually). I write the code in that order and commit to git as normal. Each functional feature I start gets a separate branch in git. So, you end up with a sequence of branches pointing to the different steps you wish to teach in the course.

Using branches is important so you can always correct any step should you find a bug that you need to fix in an earlier step (don't forget to merge forward all the way). When all branches/steps are finished, you extract each of them to a separate folder and let Itinerary detect migrations between them.
