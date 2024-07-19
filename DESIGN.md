# The architecture of CreativeDocs

This document will give you a broad overview of the current design of creativedocs.

Note: this software was written long ago and as I am not the original author of the software, I had to guess things at times. Some of my assumptions described here may be wrong. The opinions I express here are solely mine and do not necessarly reflect those of the original authors.

## Philosophy and design principles

### Avoid external dependencies

External dependencies are avoided or minimized as much as possible. This has the benefit that the software can be ported more easily. For instance, the old version of creativedocs was drawing all it's UI in a memory buffer, which was then displayed on screen in a Windows Form. This low dependency on WinForms made it easier to adapt this part of the software, simply replacing the WinForms with a cross-platform alternative (I chose to use SDL). If all the UI had been made with WinForm widgets, I would have had to replace a lot more things.

The drawback of this philosophy is that it leads to a lot of "reinventing the wheel". This additional code needs to be maintained and will generaly be less robust than a popular library that is used in hundreds of other projects. Note that said library did not necessarly exist when Creativedocs was first written.

I chose to deviate from this principle when a decent library existed for the task and implementing the functionality myself would have taken too long.

Currently, Creativedocs has the following dependencies:
- `SharpZipLib` to read and write zip files (this dependency was originaly present)
- `NativeFileDialogSharp` to have cross-platform file chooser dialog windows
- `SDL2` to create graphical windows and get user inputs
- `Imagemagick` to read and write images in different formats
- `antigrain` (via the grafix submodule) to draw various shapes
- `freetype` (via the grafix submodule) to draw text and read fonts


## Structure of the project

The project provides three similar programs:
- `App.CreativeDocs` the english version of the program
- `App.CresusDocuments` the french version of the program
- `App.CresusPictogrammes` a tool to specificaly draw icons

Most of the code is located in those folders:
- `Common` the lowest level building blocks of the app. This is mostly a library to build GUI applications.
- `Common.Document` a library of building blocks specific to Creativedocs
- `Common.DocumentEditor` the application made from the above building blocks

There are also some tests located in `Common.Tests`.


## Serialization of data
The original software used a `BinarySerializer` to serialize the application data.
This kind of serialization is difficult to debug and evolve and has been deprecated.

I started to work on a new serialization format, replacing the binary serialization with xml.

As this is a breaking change, there would be some UI work needed to warn the user when opening files in the old format.
For now, the new version only supports the new xml format.
I made a tool to convert old files to the new format, which can be found on the branch `wip/bl-format-converter`.

This work with the new format is not entirely done: the application data is serialized to xml but embedded images and fonts are currently not serialized.
Ideally, we would use a serialization format that is compatible with svg (in a similar way to inkscape). This is a first step in this direction, as svg is a specialized xml.

### Image serialization
When saving a creativedocs file, we should embed the bitmap images inside the file so that it can be opened elsewhere.
We could have a file format that is a zip of the xml application data and the bitmap images.
Another possibility would be to encode the bitmap image in base64 and store it in the xml directly (the svg format does that)

### Font serialization
As with the bitmap image, we could embed the fonts inside the file by zipping thew with the data or encode them in some way (the svg standard does not support embedding fonts, but some applications like inkscape have defined a way to embed fonts in svg)