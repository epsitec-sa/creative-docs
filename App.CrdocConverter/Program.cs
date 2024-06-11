using System.IO;
using Epsitec.Common.Document;

string root =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CreativeDocs\\Original Samples";

string outputDir =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter\\output_files";

string inputFile = "ab.crdoc";
string outputFile = "ab.xml";

Document LoadOriginalDocument(string inputFile)
{
    Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache();
    Document document = new Document(
        DocumentType.Graphic,
        DocumentMode.Modify,
        InstallType.Full,
        DebugMode.Release,
        null,
        null,
        null,
        null
    );
    Console.WriteLine("    - read old");
    document.Read(inputFile);
    return document;
}

void ExportToNewFormat(Document original, string filepath)
{
    Console.WriteLine("    - write xml");
    original.Write(filepath);
}

void CheckReadBackDocument(Document original, string filepath)
{
    Console.WriteLine("    - check reading back from xml");
    Document newDocument = Document.LoadFromXMLFile(filepath);

    System.Diagnostics.Debug.Assert(newDocument.Type == original.Type);
    System.Diagnostics.Debug.Assert(newDocument.Name == original.Name);
}

void TestConvert(string oldFile, string newFile)
{
    Console.WriteLine($"Convert {oldFile}");
    string inputFilePath = Path.Join(root, inputFile);
    string outputFilePath = Path.Join(outputDir, outputFile);
    Document originalDocument = LoadOriginalDocument(inputFilePath);
    ExportToNewFormat(originalDocument, outputFilePath);
    CheckReadBackDocument(originalDocument, outputFilePath);
}

TestConvert(inputFile, outputFile);
Console.WriteLine($"Done.");
