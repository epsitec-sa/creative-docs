using System.IO;
using Epsitec.Common.Document;

string root =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter\\old_format_files";

string outputDir =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter\\output_files";

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
    document.Read(inputFile);
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
    Document newDocument = Document.LoadFromXMLFile(filepath);
    try
    {
        newDocument.AssertIsEquivalent(original);
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine(WithColor($"    Error: {ex.Message}", 31));
        failed++;
    }
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
if (failed == 0)
{
    Console.WriteLine(WithColor($"All conversions succeeded !", 32));
}
else
{
    Console.WriteLine(WithColor($"{failed} conversions failed !", 31));
}
