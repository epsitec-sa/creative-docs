# Creativedocs improvement ideas

This is a list of possible improvements to the codebase of Creativedocs.
They are not strictly necessary for the project to work but they would make the code easier to maintain.

## Handling paths
Paths are currently handled manually with strings.
It would probably be better to use a standard library for that.
Many paths to ressources are hardcoded or relative and break easily.

## Serialization of data
We should use a safe serialization format like json or xml instead of BinarySerializer.
This would be a breaking change so there would be some UI work needed to warn the user when opening old files.

I started to work on a new serialization format (serialization to xml) on the wip/bl-format-converter branch. Ideally, we would use a serialization format that is compatible with svg (in a similar way to inkscape). This is a first step in this direction, as svg is a specialized xml.

## static constructors and Initialize()
Several classes have a static constructor that initializes some state.
The runtime calls these static constructors when needed, this is not user controlled.
However, there are many Initialize() methods (most of which are empty) that are called to trigger the static constructors in a semi-deterministic way. This makes debugging harder, as the debugger does not always manages to step into those static constructors.

## Singleton pattern with static field
There are several classes that implement a singleton pattern by having an instance of the class as a static field. Since those classes often also have a bunch of publicly settable properties, this is isomorphic to having a bunch of global variables.
This is bad for many reasons:
- it makes the code difficult to test: those singletons needs to be properly initialised
- it makes the tests non-deterministic: tests will pass/fail depending in which order they are executed because of some state shared through those singletons. If we really want to keep those singletons, we should at least provide a way to reset them.
- code using the singleton is tightly coupled to the singleton

## Implicit assumptions
(about state that should be initialized but is not checked early, fails later with NullReferenceException)

## Async
A lot of asynchronous code is poorly tested with manual waiting loops of Thread.Sleep to wait for asynchronous callback completion.

## Manual testing
Some tests are run together thourgh a RunTests method. They should be run separately by the test runner.
I refactored most of those, there is still one such instance in Common.Tests.Drawing.OpenTypeTest

## Rename classes with same name
There are several classes and files with the same name in different namespaces.
It would probably be clearer to rename them.

## Image serialization
When saving a creativedoc file, we should zip the bitmap images inside the file so that the file can be opened elsewere.