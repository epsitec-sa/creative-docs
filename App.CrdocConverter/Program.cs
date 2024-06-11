using System.IO;
using Epsitec.Common.Document;

string root =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter\\old_format_files";

string outputDir =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter\\output_files";

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

void TestConvert(string oldFile)
{
    Console.WriteLine($"Convert {Path.GetFileName(oldFile)}");
    string newFile = Path.GetFileNameWithoutExtension(oldFile) + ".xml";
    string inputFilePath = Path.Join(root, oldFile);
    string outputFilePath = Path.Join(outputDir, newFile);
    Document originalDocument = LoadOriginalDocument(inputFilePath);
    ExportToNewFormat(originalDocument, outputFilePath);
    CheckReadBackDocument(originalDocument, outputFilePath);
}

foreach (string file in Directory.GetFiles(root))
{
    TestConvert(file);
}
Console.WriteLine($"Done.");
