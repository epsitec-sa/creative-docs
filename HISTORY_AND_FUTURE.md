# History and future of Creativedocs

The first version of Creativedocs was developed at Epsitec SA between 2004 and 2014. It was designed to run on Windows only (supporting other operating systems was not really a concern at the time).

In 2024, I (Baptiste) was hired as an intern at Epsitec SA with the task to adapt Creativedocs to make it cross-platform. In our case, that meant adding support for macos and linux to target the majority of desktop computers (mobile / touch devices were not a target) and with the distant dream of running it inside a webbrowser.

During six months, I replaced Windows-specific code by cross-platform equivalents and tried to improve the overall maintainability of the codebase by cleaning up unnecessary parts, refactoring or documenting the issues I noticed. Bit after bit, I managed to get a basic prototype on which I iterated, reimplementing features one after the other.

Unfortunately, the more I worked on the codebase, the more I became aware of it's fundamental design flaws. Each change I tried to make would be more difficult and take more time than expected. Many times, refactoring one part of the code would break some other seemingly unrelated part. In the end, I have a working prototype that demonstrate the theoretical feasibility of the project, but it has still a lot of missing features, is unstable and not usable yet.

Bellow, I describe different stategies that you could take to continue this project:

## Historical conservation

**Goal**: You are like an archivst, you want to run the original software as-is on other systems, without modifying it's source code (for authenticity, but also to avoid breaking it's functionality). You have no intention to add new features or make the software evolve in any way.

**How**: The original software already runs on windows. You could try using [wine](https://www.winehq.org/) to run it on macos and linux.

## Direct port to other platforms

**Goal**: You want to port the software to new platforms by modifying it's source code. You also want to modernize the codebase to make further developments easier.

**How**: Try to understand the current code. Identify which parts you need to replace. Make your changes and possibly repair what you have accidentaly broken in the process. This is the approach I tried. If you want to take this path, be warned that it will require a tremendous amount of time, energy and skills (See [TODO.md](TODO.md) for a list of some issues with the current codebase). The third approach listed here will take you more or less to the same result with less efforts. 

## Twin from scratch

**Goal**: You want to make a new software from scratch, with the same look and feel as the original software.

**How**: List the features of the original software. Design a clean architecture for the project. Implement your architecture step by step. Reuse parts of the original code when sensible (e.g. for some specific algorithms).