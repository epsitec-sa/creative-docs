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