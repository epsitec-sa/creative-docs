
using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for PrintDialogTest.
	/// </summary>
	[TestFixture] public class PrintDialogTest
	{
		public PrintDialogTest()
		{
			Widgets.Widget.Initialize ();
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}


		
		[Test] public void CheckPrinterSettingsInstalledPrinters()
		{
			string[] printers = Printing.PrinterSettings.InstalledPrinters;
			for (int i = 0; i < printers.Length; i++)
			{
				System.Console.Out.WriteLine ("{0}: {1}", i, printers[i]);
			}
		}
		
		[Test] public void CheckShow1()
		{
			PrintDialog dialog = new PrintDialog ();

			using (Tool.InjectKey (System.Windows.Forms.Keys.Return))
			{
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

				dialog.OpenDialog ();

				System.Console.Out.WriteLine ("Paper Source: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSource.Name);
				System.Console.Out.WriteLine ("Paper Size:   {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSize.Name);
				System.Console.Out.WriteLine ("Page Bounds:  {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Bounds.ToString ());
				System.Console.Out.WriteLine ("Page Margins: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Margins.ToString ());
				System.Console.Out.WriteLine ("Output Port:  {0}", dialog.Document.PrinterSettings.OutputPort);
				System.Console.Out.WriteLine ("Driver Name:  {0}", dialog.Document.PrinterSettings.DriverName);
				System.Console.Out.WriteLine ("Collation:    {0}, {1} copies", dialog.Document.PrinterSettings.Collate, dialog.Document.PrinterSettings.Copies);
			}
		}
		
		[Test] public void CheckShow2()
		{
			PrintDialog dialog = new PrintDialog ();
			
			dialog.AllowFromPageToPage = true;
			dialog.AllowSelectedPages  = false;
			
			string[] printers = Printing.PrinterSettings.InstalledPrinters;
			
			dialog.Document.SelectPrinter (printers[printers.Length-1]);
			
			dialog.Document.PrinterSettings.MinimumPage = 1;
			dialog.Document.PrinterSettings.MaximumPage = 99;
			dialog.Document.PrinterSettings.FromPage = 5;
			dialog.Document.PrinterSettings.ToPage = 20;
			
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages  = true;
			dialog.PrintToFile         = true;
			
			dialog.Document.PrinterSettings.PrintRange = Printing.PrintRange.AllPages;
			dialog.Document.PrinterSettings.Collate = true;
			dialog.Document.PrinterSettings.Copies = 3;

			using (Tool.InjectKey (System.Windows.Forms.Keys.Return))
			{
				dialog.OpenDialog ();
			}
			
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
		
		[Test] public void CheckShow3()
		{
			if (Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment)
			{
				//	Do nothing in automated test environment, since Adope PDF requires
				//	user interaction.

				return;
			}

			PrinterDocumentPropertiesDialog dialog = new PrinterDocumentPropertiesDialog ();
			
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

			dialog.OpenDialog ();
			
			System.Console.Out.WriteLine ("Paper Source: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSource.Name);
			System.Console.Out.WriteLine ("Paper Size:   {0}", dialog.Document.PrinterSettings.DefaultPageSettings.PaperSize.Name);
			System.Console.Out.WriteLine ("Page Bounds:  {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Bounds.ToString ());
			System.Console.Out.WriteLine ("Page Margins: {0}", dialog.Document.PrinterSettings.DefaultPageSettings.Margins.ToString ());
			System.Console.Out.WriteLine ("Output Port:  {0}", dialog.Document.PrinterSettings.OutputPort);
			System.Console.Out.WriteLine ("Driver Name:  {0}", dialog.Document.PrinterSettings.DriverName);
			System.Console.Out.WriteLine ("Collation:    {0}, {1} copies", dialog.Document.PrinterSettings.Collate, dialog.Document.PrinterSettings.Copies);

			dialog.OpenDialog ();
		}
		
		[Test]
		[Ignore ("Crashes subsequent tests")]
		public void CheckPort()
		{
			Drawing.Graphics preview = new Drawing.Graphics ();
			preview.SetPixmapSize (250, 120);
			
			PrintDialogTest.Helper.TestDocument (preview);
			
			Widgets.Window window = new Widgets.Window ();
			AggPreview     widget = new AggPreview (preview);
			
			window.Text = "CheckPort - AGG";
			window.ClientSize = new Drawing.Size (250, 120);
			widget.Dock       = Widgets.DockStyle.Fill;
			widget.SetParent (window.Root);
			
			window.Show ();
			
			System.Windows.Forms.Form form;
			
			form = new System.Windows.Forms.Form ();
			form.BackColor = System.Drawing.Color.FromArgb (255, 255, 255);
			form.Text = "CheckPort/GDI+/Smooth";
			form.ClientSize = new System.Drawing.Size (250, 120);
			form.Paint += PrintDialogTest.Helper.HandleFormPaintSmooth;
			form.Show ();
			
			form = new System.Windows.Forms.Form ();
			form.BackColor = System.Drawing.Color.FromArgb (255, 255, 255);
			form.Text = "CheckPort/GDI+/Default";
			form.ClientSize = new System.Drawing.Size (250, 120);
			form.Paint += PrintDialogTest.Helper.HandleFormPaintDefault;
			form.Show ();
		}
		
		[Test] public void CheckPrint()
		{
			PrintDialog dialog = new PrintDialog ();
			
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages  = false;
			
			string[] printers = Printing.PrinterSettings.InstalledPrinters;
			
			dialog.Document.PrinterSettings.MinimumPage = 1;
			dialog.Document.PrinterSettings.MaximumPage = 1;
			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = 1;
			dialog.Document.PrinterSettings.PrintRange = Printing.PrintRange.AllPages;
			dialog.Document.PrinterSettings.Collate = false;

			using (Tool.InjectKey (System.Windows.Forms.Keys.Return))
			{
				dialog.OpenDialog ();
			}
			
			if (dialog.Result == Dialogs.DialogResult.Accept)
			{
				dialog.Document.Print (new PrintEngine ());
			}
		}
		
		[Test] public void CheckPrintToFile()
		{
			PrintDialog dialog = new PrintDialog ();
			
			dialog.Document.SelectPrinter ("Adobe PDF");
			
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages  = false;
			
			string[] printers = Printing.PrinterSettings.InstalledPrinters;

			using (Tool.InjectKey (System.Windows.Forms.Keys.Return))
			{
				dialog.Document.PrinterSettings.MinimumPage = 1;
				dialog.Document.PrinterSettings.MaximumPage = 1;
				dialog.Document.PrinterSettings.FromPage = 1;
				dialog.Document.PrinterSettings.ToPage = 1;
				dialog.Document.PrinterSettings.PrintRange = Printing.PrintRange.AllPages;
				dialog.Document.PrinterSettings.Collate = false;
				dialog.Document.PrinterSettings.PrintToFile = true;
				dialog.Document.PrinterSettings.OutputFileName = @"c:\test.ps";
				dialog.Document.Print (new PrintEngine ());
			}

			try
			{
				Adobe.AcrobatDistiller.PdfDistiller distiller = new Adobe.AcrobatDistiller.PdfDistiller ();
				distiller.bShowWindow = 0;
				distiller.OnJobStart += PrintDialogTest.HandleDistillerOnJobStart;
				distiller.OnJobDone  += PrintDialogTest.HandleDistillerOnJobDone;
				distiller.FileToPDF (@"c:\test.ps", @"c:\auto-generated-test.pdf", @"");
				System.Diagnostics.Debug.WriteLine ("Done.");
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Acrobat Distiller failed:");
				System.Diagnostics.Debug.WriteLine ("  " + ex.Message);
			}
		}
		
		private static void HandleDistillerOnJobStart(string strInputPostScript, string strOutputPDF)
		{
			System.Diagnostics.Debug.WriteLine ("JobStart: " + strOutputPDF);
		}
			
		private static void HandleDistillerOnJobDone(string strInputPostScript, string strOutputPDF)
		{
			System.Diagnostics.Debug.WriteLine ("JobDone: " + strOutputPDF);
		}
			
		
		
		protected class AggPreview : Widgets.Widget
		{
			public AggPreview(Drawing.Graphics port)
			{
				this.port = port;
			}
			
			protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
			{
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (250, 120, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
				
				using (bitmap)
				{
					Drawing.Pixmap.RawData src = new Drawing.Pixmap.RawData (this.port.Pixmap);
					Drawing.Pixmap.RawData dst = new Drawing.Pixmap.RawData (bitmap, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
					
					using (src)
					{
						using (dst)
						{
							src.CopyTo (dst);
						}
					}
					
					graphics.PaintImage (Drawing.Bitmap.FromNativeBitmap (bitmap), 0, 0, 250, 120, 0, 0, 250, 120);
				}
			}

			
			Drawing.Graphics					port;
		}
		
		protected class PrintEngine : Printing.IPrintEngine
		{
			#region IPrintEngine Members
			public void PrepareNewPage(Epsitec.Common.Printing.PageSettings settings)
			{
				settings.Margins = new Drawing.Margins (0, 0, 0, 0);
			}
			
			public void FinishingPrintJob()
			{
			}
			
			public void StartingPrintJob()
			{
			}
			
			public Printing.PrintEngineStatus PrintPage(Printing.PrintPort port)
			{
				//?Drawing.Font font = Drawing.Font.GetFont ("Arial", "Regular");
				Drawing.Font font = Drawing.Font.GetFont ("Tahoma", "Italic");
				
				port.LineWidth = 0.1;
				port.Color     = Drawing.Color.FromRgb (0, 0, 0);
				
				for (int x = 0; x < 210; x++)
				{
					double y = 6;
					if ((x % 5) == 0)
					{
						y = 8;
						port.PaintText (x, 10, string.Format ("{0}", x), font, 1.2);
					}
					
					port.PaintOutline (Drawing.Path.FromLine (x, 5, x, y));
				}
				
				for (int y = 0; y < 297; y++)
				{
					double x = 6;
					if ((y % 5) == 0)
					{
						x = 8;
						port.PaintText (10, y, string.Format ("{0}", y), font, 1.2);
					}
					
					port.PaintOutline (Drawing.Path.FromLine (5, y, x, y));
				}
				
				Drawing.Image bitmap = Drawing.Bitmap.FromFile (@"..\..\Images\picture.jpg");
				
				port.PaintText (50, 145, "Image de Délos, définie à 300 dpi, 100mm x 100mm, codé au format JPEG.", font, 2.5);
				port.PaintText (50, 141, "Imprimé avec les mécanismes de bas niveau de Crésus Réseau.", font, 2.5);
				port.PaintImage (bitmap, 50, 150, 100, 100, 0, 0, bitmap.Width, bitmap.Height);
				
				PrintDialogTest.Helper.TestDocument (port);
				
				return Printing.PrintEngineStatus.FinishJob;
			}
			#endregion
		}
		
		private class Helper
		{
			public static void TestDocument(Drawing.IPaintPort port)
			{
#if true
				Drawing.Font font_1 = Drawing.Font.GetFont ("Tahoma", "Regular");
				//?Drawing.Font font_1 = Drawing.Font.GetFont ("Arial", "Regular");
				Drawing.Font font_2 = Drawing.Font.GetFont ("Times New Roman", "Regular");
				Drawing.Font font_3 = Drawing.Font.GetFont ("Times New Roman", "Italic");
				
				//			port.RotateTransform (15, 125, 60);
				
				Drawing.Path path = new Drawing.Path ();
				
				path.MoveTo (10, 10);
				path.CurveTo (10, 20, 20, 30, 30, 30);
				path.CurveTo (30, 20, 20, 10, 10, 10);
				path.Close ();
				
				for (int i = 0; i < 2; i++)
				{
					port.Color     = Drawing.Color.FromRgb (0, 0, 0);
					port.LineCap   = Drawing.CapStyle.Square;
					port.LineJoin  = Drawing.JoinStyle.MiterRevert;
					port.LineWidth = 1.5;
					
					port.PaintOutline (path);
					port.PaintOutline (Drawing.Path.FromLine (40, 10, 60, 20));
					port.PaintOutline (Drawing.Path.FromCircle (40, 30, 5));
					
					port.TranslateTransform (60, 0);
					
					port.Color     = Drawing.Color.FromRgb (0, 0, 0);
					port.LineCap   = Drawing.CapStyle.Square;
					port.LineJoin  = Drawing.JoinStyle.MiterRevert;
					port.LineWidth = 0.5;
					
					port.PaintOutline (path);
					port.PaintOutline (Drawing.Path.FromLine (40, 10, 60, 20));
					port.PaintOutline (Drawing.Path.FromCircle (40, 30, 5));
					
					port.TranslateTransform (60, 0);
					
					port.Color = Drawing.Color.FromRgb (0, 1, 0);
					
					port.PaintSurface (path);
					port.PaintSurface (Drawing.Path.FromLine (40, 10, 60, 20));
					port.PaintSurface (Drawing.Path.FromCircle (40, 30, 5));
					
					port.TranslateTransform (60, 0);
					
					port.Color = Drawing.Color.FromRgb (1, 1, 0);
					
					port.PaintSurface (path);
					port.PaintSurface (Drawing.Path.FromLine (40, 10, 60, 20));
					port.PaintSurface (Drawing.Path.FromCircle (40, 30, 5));
					
					port.Color     = Drawing.Color.FromRgb (1, 0, 0);
					port.LineCap   = Drawing.CapStyle.Square;
					port.LineJoin  = Drawing.JoinStyle.MiterRevert;
					port.LineWidth = 1.0;
					
					port.PaintOutline (path);
					port.PaintOutline (Drawing.Path.FromLine (40, 10, 60, 20));
					port.PaintOutline (Drawing.Path.FromCircle (40, 30, 5));
					
					port.TranslateTransform (-180, 0);
					
					port.Color = Drawing.Color.FromRgb (0, 0, 0);
					
					double ox = 10;
					
					ox += port.PaintText (ox, 40, "Test document: ", font_1, 12);
					ox += port.PaintText (ox, 40, " Times New Roman ", font_2, 12);
					ox += port.PaintText (ox, 40, "Italic.", font_3, 12);

					port.LineWidth = 0.2;
					port.PaintOutline (Drawing.Path.FromLine ( 5, 40, 15, 40));
					port.PaintOutline (Drawing.Path.FromLine (10, 35, 10, 45));
					
					if (i == 0)
					{
						port.TranslateTransform (0, 60);
						port.SetClippingRectangle (new Drawing.Rectangle (40, 80, 160, 25));
					}
				}
#else
				Drawing.Font font_1 = Drawing.Font.GetFont ("Arial", "Regular");
				Drawing.Font font_2 = Drawing.Font.GetFont ("Times New Roman", "Regular");
				Drawing.Font font_3 = Drawing.Font.GetFont ("Times New Roman", "Italic");
				
				Printing.PrintPort pport = port as Printing.PrintPort;
				
				port.LineWidth = 0.2;
				port.PaintOutline (Drawing.Path.FromLine (35, 40, 45, 40));
				port.PaintOutline (Drawing.Path.FromLine (40, 35, 40, 45));
				
				port.Color = Drawing.Color.FromRgb (0, 0, 0);
				pport.PaintText (40, 40, "A", font_1, 100.0, true);
				port.Color = Drawing.Color.FromRgb (0, 1, 0);
				pport.PaintText (40, 40, "X", font_2, 100.0, true);
				port.Color = Drawing.Color.FromRgb (1, 1, 0);
				pport.PaintText (40, 40, "Y", font_3, 100.0, true);
				
				port.Color = Drawing.Color.FromRgb (0, 0, 0);
				port.PaintOutline (Drawing.Path.FromLine (35, 160, 45, 160));
				port.PaintOutline (Drawing.Path.FromLine (40, 155, 40, 165));
				
				port.Color = Drawing.Color.FromRgb (0, 0, 0);
				pport.PaintText (40, 160, "A", font_1, 100.0, false);
				port.Color = Drawing.Color.FromRgb (0, 1, 0);
				pport.PaintText (40, 160, "X", font_2, 100.0, false);
				port.Color = Drawing.Color.FromRgb (1, 1, 0);
				pport.PaintText (40, 160, "Y", font_3, 100.0, false);
#endif
			}
			
			
			public static void HandleFormPaintSmooth(object sender, System.Windows.Forms.PaintEventArgs e)
			{
				System.Windows.Forms.Form form = sender as System.Windows.Forms.Form;
				
				e.Graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				
				Printing.PrintPort port = new Printing.PrintPort (e.Graphics, form.ClientSize.Width, form.ClientSize.Height);
				PrintDialogTest.Helper.TestDocument (port);
			}
			
			public static void HandleFormPaintDefault(object sender, System.Windows.Forms.PaintEventArgs e)
			{
				System.Windows.Forms.Form form = sender as System.Windows.Forms.Form;
				
				e.Graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.Default;
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
				
				Printing.PrintPort port = new Printing.PrintPort (e.Graphics, form.ClientSize.Width, form.ClientSize.Height);
				PrintDialogTest.Helper.TestDocument (port);
			}
		}

	}
}
