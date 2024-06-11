using System.IO;
using Epsitec.Common.Document;

string root =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CreativeDocs\\Original Samples";

string outputDir =
    "C:\\devel\\cresus-core-dev-converter\\cresus-core\\App.CrdocConverter\\output_files";

string inputFile = "ab.crdoc";
string outputFile = "ab.xml";

string inputFilePath = Path.Join(root, inputFile);
string outputFilePath = Path.Join(outputDir, outputFile);
Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache();
Document doc = new Document(
    DocumentType.Graphic,
    DocumentMode.Modify,
    InstallType.Full,
    DebugMode.Release,
    null,
    null,
    null,
    null
);
doc.Read(inputFilePath);
doc.Write(outputFilePath);
