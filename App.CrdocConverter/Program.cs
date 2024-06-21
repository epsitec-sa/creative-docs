using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Epsitec.Common.Document;
using Objects = Epsitec.Common.Document.Objects;

string root = "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter";
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
    string outputFilePath = Path.Join(outputDir, newFile);
    Document originalDocument = LoadOriginalDocument(oldFile);
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
}

//DebugBinaryFormatter();
TestConvert(Path.Join(inputDir, "Save.icon"));
//ConvertAllFiles();
