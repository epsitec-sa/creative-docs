using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for PrintDialogTest.
	/// </summary>
	[TestFixture] public class PrintDialogTest
	{
		[Test] public void CheckPrinterSettingsInstalledPrinters()
		{
			string[] printers = Printing.PrinterSettings.InstalledPrinters;
			for (int i = 0; i < printers.Length; i++)
			{
				System.Console.Out.WriteLine ("{0}: {1}", i, printers[i]);
			}
		}
		
		[Test] public void CheckShow()
		{
			Print dialog = new Print ();
			
			dialog.AllowFromPageToPage = true;
			dialog.AllowSelectedPages  = false;
			
			string[] printers = Printing.PrinterSettings.InstalledPrinters;
			
			dialog.Document.SelectPrinter (printers[printers.Length-1]);
			
			dialog.Document.PrinterSettings.MinimumPage = 1;
			dialog.Document.PrinterSettings.MaximumPage = 99;
			dialog.Document.PrinterSettings.FromPage = 5;
			dialog.Document.PrinterSettings.ToPage = 20;
			dialog.Document.PrinterSettings.PrintRange = Printing.PrintRange.FromPageToPage;
			dialog.Document.PrinterSettings.Collate = false;
			
			dialog.Show ();
			
			System.Console.Out.WriteLine ("Paper Source: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSource.Name);
			System.Console.Out.WriteLine ("Paper Size:   {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSize.Name);
			System.Console.Out.WriteLine ("Page Bounds:  {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Bounds.ToString ());
			System.Console.Out.WriteLine ("Page Margins: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Margins.ToString ());
			System.Console.Out.WriteLine ("Output Port:  {0}", dialog.Document.PrinterSettings.OutputPort);
			System.Console.Out.WriteLine ("Driver Name:  {0}", dialog.Document.PrinterSettings.DriverName);
			
			
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages  = true;
			dialog.PrintToFile         = true;
			
			dialog.Document.PrinterSettings.PrintRange = Printing.PrintRange.AllPages;
			dialog.Document.PrinterSettings.Collate = true;
			dialog.Document.PrinterSettings.Copies = 3;
			
			dialog.Show ();
			
			System.Console.Out.WriteLine ("Paper Source: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSource.Name);
			System.Console.Out.WriteLine ("Paper Size:   {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSize.Name);
			System.Console.Out.WriteLine ("Page Bounds:  {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Bounds.ToString ());
			System.Console.Out.WriteLine ("Page Margins: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Margins.ToString ());
			System.Console.Out.WriteLine ("Output Port:  {0}", dialog.Document.PrinterSettings.OutputPort);
			System.Console.Out.WriteLine ("Driver Name:  {0}", dialog.Document.PrinterSettings.DriverName);
			
			Printing.PaperSource[]       sources = dialog.Document.PrinterSettings.PaperSources;
			Printing.PaperSize[]         sizes   = dialog.Document.PrinterSettings.PaperSizes;
			Printing.PrinterResolution[] resols  = dialog.Document.PrinterSettings.PrinterResolutions;
			
			System.Console.Out.WriteLine ("Paper Sources:");
			
			for (int i = 0; i < sources.Length; i++)
			{
				System.Console.Out.WriteLine ("  {0}: {1} = {2}", i, sources[i].Kind.ToString (), sources[i].Name);
			}
			
			System.Console.Out.WriteLine ("Paper Sizes:");
			
			for (int i = 0; i < sizes.Length; i++)
			{
				System.Console.Out.WriteLine ("  {0}: {1} = {2}, {3}", i, sizes[i].Kind.ToString (), sizes[i].Name, sizes[i].Size.ToString ());
			}
			
			System.Console.Out.WriteLine ("Printer Resolutions:");
			
			for (int i = 0; i < resols.Length; i++)
			{
				System.Console.Out.WriteLine ("  {0}: {1} x {2}", i, resols[i].DpiX, resols[i].DpiY);
			}
		}
	}
}
