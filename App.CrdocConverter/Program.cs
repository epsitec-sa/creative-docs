using System.IO;
using Epsitec.Common.Document;

string root = "C:\\Users\\Baptiste\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter";
string inputDir = Path.Join(root, "old_format_files");
string outputDir = Path.Join(root, "output_files");

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
    string inputFilePath = Path.Join(inputDir, oldFile);
    string outputFilePath = Path.Join(outputDir, newFile);
    Document originalDocument = LoadOriginalDocument(inputFilePath);
    ExportToNewFormat(originalDocument, outputFilePath);
    CheckReadBackDocument(originalDocument, outputFilePath);
}

foreach (string file in Directory.GetFiles(inputDir))
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
