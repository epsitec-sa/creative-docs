# Creativedocs TODO
This is a list of possible improvements to the codebase of Creativedocs.
They are not strictly necessary for the project to work but they would make the code easier to maintain.

## Handling paths
Paths are currently handled manually with strings.
It would probably be better to use a standard library for that.
Many paths to ressources are hardcoded or relative and break easily.

## Serialization of data
We should use a safe serialization format like json instead of BinarySerializer.
This would be a breaking change so there would be some UI work needed to warn the user when opening old files.

## static constructors and Initialize()
Several classes have a static constructor that initializes some state.
The runtime calls these static constructors when needed, this is not user controlled.
However, there are many Initialize() methods (most of which are empty) that are called to trigger the static constructors in a semi-deterministic way. This makes debugging harder, as the debugger does not always manages to step into those static constructors.

## Async
A lot of asynchronous code is poorly tested with manual waiting loops of Thread.Sleep to wait for asynchronous callback completion.