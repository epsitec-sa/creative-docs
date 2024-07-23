/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

﻿using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Epsitec.Common.Document;
using Objects = Epsitec.Common.Document.Objects;

string root = "C:\\devel\\cresus-core-dev-converter\\cresus-core";

List<string> imageDirectories =
[
    "Common\\Widgets\\Images",
    "Common\\Dialogs\\Images",
    "Common.Tests\\Images",
    "Common.DocumentEditor\\Images",
];

string debugOutputDir = Path.Join(root, "App.CrdocConverter\\output_files");

bool verbose = false;
int failed = 0;

string WithColor(string message, int color)
{
    return $"\x1b[{color}m{message}\x1b[0m";
}

Document LoadOriginalDocument(string inputFile)
{
    Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache();
    Document document = new Document(
        Path.GetExtension(inputFile) == ".icon" ? DocumentType.Pictogram : DocumentType.Graphic,
        DocumentMode.Modify,
        InstallType.Full,
        DebugMode.Release,
        null,
        null,
        null,
        null
    );
    if (verbose)
    {
        Console.WriteLine("    - read old");
    }
    string error = document.Read(inputFile);
    if (error != "")
    {
        throw new System.InvalidOperationException(error);
    }
    return document;
}

void ExportToNewFormat(Document original, string filepath)
{
    if (verbose)
    {
        Console.WriteLine("    - write xml");
    }
    original.Write(filepath);
}

void CheckReadBackDocument(Document original, string filepath)
{
    if (verbose)
    {
        Console.WriteLine("    - check reading back from xml");
    }
    Document newDocument = Document.LoadFromXMLFile(filepath, DocumentMode.Modify);
    if (!newDocument.HasEquivalentData(original))
    {
        Console.WriteLine(WithColor($"    Error: ", 31));
        failed++;
    }
}

void TestConvert(string oldFile)
{
    Console.WriteLine($"Convert {Path.GetFileName(oldFile)}");
    string newFile = Path.GetFileNameWithoutExtension(oldFile) + ".xml";
    string outputFilePath = Path.Join(Path.GetDirectoryName(oldFile), newFile);
    string debugOutputFilePath = Path.Join(debugOutputDir, newFile);
    Document originalDocument = LoadOriginalDocument(oldFile);
    ExportToNewFormat(originalDocument, debugOutputFilePath);
    ExportToNewFormat(originalDocument, outputFilePath);
    CheckReadBackDocument(originalDocument, outputFilePath);
}

void DebugBinaryFormatter()
{
    Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache();
    var doc = new Document(
        DocumentType.Pictogram,
        DocumentMode.ReadOnly,
        InstallType.Full,
        DebugMode.Release,
        null,
        null,
        null,
        null
    );
    Document.ReadDocument = doc;
    var data = new UndoableList(doc, UndoableListType.ObjectsInsideDocument);
    var page = new Objects.Page(doc, null);
    data.Add(page);

    using (Stream stream = new MemoryStream())
    {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, data);

        stream.Position = 0;

        BinaryFormatter bformatter = new BinaryFormatter();
        var data_back = (UndoableList)bformatter.Deserialize(stream);
        //if (data.HasEquivalentData(data_back))
        var a = (Objects.Page)data[0];
        var b = (Objects.Page)data_back[0];
        if (a.HasEquivalentData(b))
        {
            Console.WriteLine("Data matches.");
        }
        else
        {
            Console.WriteLine("Data DOES NOT MATCH !!!");
        }
    }
}

void ConvertAllFiles()
{
    failed = 0;
    foreach (string directoryName in imageDirectories)
    {
        string directory = Path.Join(root, directoryName);
        Console.WriteLine($"Convert files in directory {directory}");
        foreach (string file in Directory.GetFiles(directory, "*.icon"))
        {
            TestConvert(file);
        }
    }
    if (failed == 0)
    {
        Console.WriteLine(WithColor($"All conversions succeeded !", 32));
    }
    else
    {
        Console.WriteLine(WithColor($"{failed} conversions failed !", 31));
    }
}

//DebugBinaryFormatter();
//TestConvert(Path.Join(inputDir, "aqua1.crdoc"));
ConvertAllFiles();
