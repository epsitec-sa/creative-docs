# Creativedocs

## Overview

Creativedocs is a vector graphics editor originaly developed at Epsitec SA between 2004 and 2014.
It supports creating images from simple vector shapes, adding text and bitmap images, multiple layers and pages and various export formats.

![Screenshot of the interface of Creativedocs](./creativedocs_new_interface.png)

This open-source release is an attempt to modernise this software and port it to new platforms (macos and linux).

## Project status

This project is not in active development.

## Building

- clone this repository and it's submodules
- follow the build instructions in `grafix`
- open `cresus-core/App.CreativeDocs` and build `App.CreativeDocs.sln` with Visual Studio or from the command line `dotnet build App.CreativeDocs.csproj`
- the other projects (`App.CresusDocuments` and `App.CresusPictogrammes`) can be built in a similar way

## Running

- run from Visual Studio or from the command line `dotnet run App.CreativeDocs.csproj`

## Contributing

If you are very motivated and want to build on this project, you might find the following documents usefull:

- [HISTORY_AND_FUTURE.md](HISTORY_AND_FUTURE.md) a summary of the history of creativedocs and a few guidelines on how to adapt it for the future
- [DESIGN.md](DESIGN.md) an overview of the main components that make creativedocs
- [TODO.md](TODO.md) a list of improvement ideas you could work on

## Authors and acknowledgment

The original software was developed at Epsitec SA, mainly by Pierre Arnaud and Daniel Roux, between 2004 and 2014.
In 2024, an attempt to port the software for macos and linux was started by Baptiste Lambert with the help of Roger Vuistiner.

## License