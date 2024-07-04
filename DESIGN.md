# The architecture of CreativeDocs

This document will give you a broad overview of the design of creativedocs.

Note: this software was written long ago and as I am not the original author of the software, I had to guess things at times. Some of my assumptions described here may be wrong. The opinions I express here are solely mine and do not necessarly reflect those of the original authors.

## Philosophy and design principles

### No external dependencies

External dependencies are avoided or minimized as much as possible. This has the benefit that the software can be ported more easily. For instance, the old version of creativedocs was drawing all it's UI in a memory buffer, which was then displayed on screen in a Windows Form. This low dependency on WinForms made it easier to adapt this part of the software, simply replacing the WinForms with a cross-platform alternative (I chose to use SDL). If all the UI had been made with WinForm widgets, I would have had to replace a lot more things.

The drawback of this philosophy is that it leads to a lot of "reinventing the wheel". This additional code needs to be maintained and will generaly be less robust than a popular library that is used in hundreds of other projects. Note that said library did not necessarly exist when Creativedocs was first written.


## Structure of the project

The project provides three similar programs:
- `App.CreativeDocs` the english version of the program
- `App.CresusDocuments` the french version of the program
- `App.CresusPictogrammes` a tool to specificaly draw icons

Most of the code is located in those folders:
- `Common` the lowest level building blocks of the app. This is mostly a library to build GUI applications.
- `Common.Document` a library of building blocks specific to Creativedocs
- `Common.DocumentEditor` the application made from the above building blocks
