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
		
		[Test] public void CheckPort()
		{
			Widgets.Widget.Initialise ();
			
			Drawing.Agg.Graphics preview = new Drawing.Agg.Graphics ();
			preview.SetPixmapSize (250, 120);
			
			PrintDialogTest.TestDocument (preview);
			
			Widgets.Window window = new Widgets.Window ();
			AggPreview     widget = new AggPreview (preview);
			
			window.Text = "CheckPort - AGG";
			window.ClientSize = new Drawing.Size (250, 120);
			widget.Dock       = Widgets.DockStyle.Fill;
			widget.Parent     = window.Root;
			
			window.Show ();
			
			System.Windows.Forms.Form form;
			
			form = new System.Windows.Forms.Form ();
			form.BackColor = System.Drawing.Color.FromArgb (255, 255, 255);
			form.Text = "CheckPort/GDI+/Smooth";
			form.ClientSize = new System.Drawing.Size (250, 120);
			form.Paint += new System.Windows.Forms.PaintEventHandler (HandleFormPaintSmooth);
			form.Show ();
			
			form = new System.Windows.Forms.Form ();
			form.BackColor = System.Drawing.Color.FromArgb (255, 255, 255);
			form.Text = "CheckPort/GDI+/Default";
			form.ClientSize = new System.Drawing.Size (250, 120);
			form.Paint += new System.Windows.Forms.PaintEventHandler (HandleFormPaintDefault);
			form.Show ();
		}
		
		protected class AggPreview : Widgets.Widget
		{
			public AggPreview(Drawing.Agg.Graphics port)
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

			
			Drawing.Agg.Graphics				port;
		}
		
		private static void TestDocument(Drawing.IPaintPort port)
		{
			Drawing.Font font_1 = Drawing.Font.GetFont ("Tahoma", "Regular");
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
				port.Color     = Drawing.Color.FromRGB (0, 0, 0);
				port.LineCap   = Drawing.CapStyle.Square;
				port.LineJoin  = Drawing.JoinStyle.Miter;
				port.LineWidth = 1.5;
				
				port.PaintOutline (path);
				port.PaintOutline (Drawing.Path.FromLine (40, 10, 60, 20));
				port.PaintOutline (Drawing.Path.FromCircle (40, 30, 5));
				
				port.TranslateTransform (60, 0);
				
				port.Color     = Drawing.Color.FromRGB (0, 0, 0);
				port.LineCap   = Drawing.CapStyle.Square;
				port.LineJoin  = Drawing.JoinStyle.Miter;
				port.LineWidth = 0.5;
				
				port.PaintOutline (path);
				port.PaintOutline (Drawing.Path.FromLine (40, 10, 60, 20));
				port.PaintOutline (Drawing.Path.FromCircle (40, 30, 5));
				
				port.TranslateTransform (60, 0);
				
				port.Color = Drawing.Color.FromRGB (0, 1, 0);
				
				port.PaintSurface (path);
				port.PaintSurface (Drawing.Path.FromLine (40, 10, 60, 20));
				port.PaintSurface (Drawing.Path.FromCircle (40, 30, 5));
				
				port.TranslateTransform (60, 0);
				
				port.Color = Drawing.Color.FromRGB (1, 1, 0);
				
				port.PaintSurface (path);
				port.PaintSurface (Drawing.Path.FromLine (40, 10, 60, 20));
				port.PaintSurface (Drawing.Path.FromCircle (40, 30, 5));
				
				port.Color     = Drawing.Color.FromRGB (1, 0, 0);
				port.LineCap   = Drawing.CapStyle.Square;
				port.LineJoin  = Drawing.JoinStyle.Miter;
				port.LineWidth = 1.0;
				
				port.PaintOutline (path);
				port.PaintOutline (Drawing.Path.FromLine (40, 10, 60, 20));
				port.PaintOutline (Drawing.Path.FromCircle (40, 30, 5));
				
				port.TranslateTransform (-180, 0);
				
				port.Color = Drawing.Color.FromRGB (0, 0, 0.5);
				
				double ox = 10;
				
				ox += port.PaintText (ox, 40, "Test document: ", font_1, 12);
				ox += port.PaintText (ox, 40, " Times New Roman ", font_2, 12);
				ox += port.PaintText (ox, 40, "Italic.", font_3, 12);
				
				if (i == 0)
				{
					port.TranslateTransform (0, 60);
					port.SetClippingRectangle (40, 80, 160, 25);
				}
			}
		}

		private static void HandleFormPaintSmooth(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			System.Windows.Forms.Form form = sender as System.Windows.Forms.Form;
			
			e.Graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			
			Printing.PrintPort port = new Printing.PrintPort (e.Graphics, form.ClientSize.Width, form.ClientSize.Height);
			PrintDialogTest.TestDocument (port);
		}
		
		private static void HandleFormPaintDefault(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			System.Windows.Forms.Form form = sender as System.Windows.Forms.Form;
			
			e.Graphics.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.Default;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
			
			Printing.PrintPort port = new Printing.PrintPort (e.Graphics, form.ClientSize.Width, form.ClientSize.Height);
			PrintDialogTest.TestDocument (port);
		}
	}
}
