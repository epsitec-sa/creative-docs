using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Printer implémente l'impression d'un document.
	/// </summary>
	public class Printer
	{
		public Printer(Document document)
		{
			this.document = document;

			this.ImageFormat = ImageFormat.Unknown;
			this.imageDpi = 100;
			this.imageCompression = ImageCompression.None;
			this.imageDepth = 24;
			this.imageQuality = 0.85;
			this.imageAA = 1.0;
		}

		// Sauvegarde les réglages d'impression.
		public void SaveSettings(PrinterSettings settings)
		{
			if ( this.settingsMemory == null )
			{
				this.settingsMemory = new SettingsMemory(this.document);
			}

			this.settingsMemory.Save(settings);
		}

		// Restitue les réglages d'impression.
		public void RestoreSettings(PrinterSettings settings)
		{
			if ( this.settingsMemory == null )
			{
				this.settingsMemory = new SettingsMemory(this.document);
			}

			this.settingsMemory.Restore(settings);
		}

		// Imprime le document selon les choix faits dans le dialogue Window (dp)
		// ainsi que dans le dialogue des réglages (PrintInfo).
		public void Print(Epsitec.Common.Dialogs.Print dp)
		{
			PrintEngine printEngine = new PrintEngine();
			printEngine.Initialise(this, dp);
			dp.Document.Print(printEngine);
		}

		// Exporte le document dans un fichier.
		public string Export(string filename)
		{
			// Crée le DrawingContext utilisé pour l'exportation.
			DrawingContext drawingContext;
			drawingContext = new DrawingContext(this.document, null);
			drawingContext.ContainerSize = this.document.Size;
			drawingContext.PreviewActive = true;

			return this.ExportGeometry(drawingContext, filename);
		}


		#region Image
		// Trouve le type d'une image en fonction de l'extension.
		public static ImageFormat GetImageFormat(string ext)
		{
			switch ( ext.ToLower() )
			{
				case "bmp":  return ImageFormat.Bmp;
				case "gif":  return ImageFormat.Gif;
				case "tif":  return ImageFormat.Tiff;
				case "jpg":  return ImageFormat.Jpeg;
				case "png":  return ImageFormat.Png;
				case "exf":  return ImageFormat.Exif;
				case "emf":  return ImageFormat.WindowsEmf;
				case "wmf":  return ImageFormat.WindowsWmf;
			}
			return ImageFormat.Unknown;
		}

		public ImageFormat ImageFormat
		{
			get { return this.imageFormat; }
			set { this.imageFormat = value; }
		}

		public double ImageDpi
		{
			get { return this.imageDpi; }
			set { this.imageDpi = value; }
		}

		public ImageCompression ImageCompression
		{
			get { return this.imageCompression; }
			set { this.imageCompression = value; }
		}

		public int ImageDepth
		{
			get { return this.imageDepth; }
			set { this.imageDepth = value; }
		}

		public double ImageQuality
		{
			get { return this.imageQuality; }
			set { this.imageQuality = value; }
		}

		public double ImageAA
		{
			get { return this.imageAA; }
			set { this.imageAA = value; }
		}
		#endregion


		protected class PrintEngine : Printing.IPrintEngine
		{
			public void Initialise(Printer printer, Epsitec.Common.Dialogs.Print dp)
			{
				this.printer = printer;
				this.document = printer.document;

				// Crée le DrawingContext utilisé pour l'impression.
				this.drawingContext = new DrawingContext(this.document, null);
				this.drawingContext.ContainerSize = this.document.Size;
				this.drawingContext.PreviewActive = true;

				// Reprend ici tous les choix effectués dans le dialogue Window
				// de l'impression. Même s'il semble possible de les atteindre
				// plus tard avec port.PageSettings.PrinterSettings, cela
				// fonctionne mal.
				this.fromPage = dp.Document.PrinterSettings.FromPage;
				this.toPage   = dp.Document.PrinterSettings.ToPage;
				this.copies   = dp.Document.PrinterSettings.Copies;
				this.collate  = dp.Document.PrinterSettings.Collate;
				this.duplex   = dp.Document.PrinterSettings.Duplex;
				this.totalPages = this.toPage-this.fromPage+1;
				this.pageCounter = 0;
				this.multicopyByPrinter = false;

				if ( this.collate == false &&
					 this.duplex == DuplexMode.Simplex &&
					 this.copies > 1 )
				{
					this.multicopyByPrinter = true;
				}

				if ( this.multicopyByPrinter )
				{
					this.copies = 1;
				}
				else
				{
					dp.Document.PrinterSettings.Copies = 1;
				}
			}

			#region IPrintEngine Members
			public void PrepareNewPage(Printing.PageSettings settings)
			{
				//?System.Diagnostics.Debug.WriteLine("PrepareNewPage");
				settings.Margins = new Margins(0, 0, 0, 0);

				if ( this.printer.PrintInfo.AutoLandscape )
				{
					settings.Landscape = (this.document.Size.Width > this.document.Size.Height);
				}
			}
			
			public void StartingPrintJob()
			{
				//?System.Diagnostics.Debug.WriteLine("StartingPrintJob");
			}
			
			public void FinishingPrintJob()
			{
				//?System.Diagnostics.Debug.WriteLine("FinishingPrintJob");
			}
			
			public Printing.PrintEngineStatus PrintPage(Printing.PrintPort port)
			{
				int page = this.pageCounter;

				if ( this.collate )  // 1,2,3,4 - 1,2,3,4 ?
				{
					page %= this.totalPages;
				}
				else
				{
					if ( this.duplex != DuplexMode.Simplex )  // 1,2 - 1,2 - 3,4 - 3,4 ?
					{
						     if ( (page&0x3) == 0x1 )  page = (page&~0x3)|0x2;
						else if ( (page&0x3) == 0x2 )  page = (page&~0x3)|0x1;
						page /= this.copies;
					}
					else	// 1,1 - 2,2 - 3,3 - 4,4 ?
					{
						page /= this.copies;
					}
				}
				page += this.fromPage-1;
				//?System.Diagnostics.Debug.WriteLine("PrintPage "+page.ToString());
				this.printer.PrintGeometry(port, this.drawingContext, page);
				this.pageCounter ++;

				if ( this.pageCounter < this.totalPages*this.copies )
				{
					return Printing.PrintEngineStatus.MorePages;
				}
				return Printing.PrintEngineStatus.FinishJob;
			}
			#endregion

			protected Document				document;
			protected Printer				printer;
			protected DrawingContext		drawingContext;
			protected int					pageCounter;
			protected int					fromPage;
			protected int					toPage;
			protected int					totalPages;
			protected int					copies;
			protected bool					collate;
			protected DuplexMode			duplex;
			protected bool					multicopyByPrinter;
		}


		// Imprime la géométrie de tous les objets.
		protected void PrintGeometry(Printing.PrintPort port,
									 DrawingContext drawingContext,
									 int pageNumber)
		{
			System.Diagnostics.Debug.Assert(pageNumber >= 0);
			System.Diagnostics.Debug.Assert(pageNumber < this.document.GetObjects.Count);

			if ( this.PrintInfo.ForceSimply )
			{
				this.PrintSimplyGeometry(port, drawingContext, pageNumber, false);
			}
			else if ( this.PrintInfo.ForceComplex )
			{
				Rectangle clipRect = new Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
				this.PrintComplexGeometry(port, drawingContext, pageNumber, clipRect);
			}
			else
			{
#if true
				this.PrintSimplyGeometry(port, drawingContext, pageNumber, true);

				System.Collections.ArrayList areas = this.ComputeAreas(pageNumber);
				foreach ( PrintingArea area in areas )
				{
					if ( area.IsComplex )
					{
						this.PrintComplexGeometry(port, drawingContext, pageNumber, area.Area);
					}
				}

				if ( this.PrintInfo.DebugArea )
				{
					this.PrintAreas(port, drawingContext, areas);
				}
#else
				if ( this.IsComplexPrinting(pageNumber) )
				{
					Rectangle clipRect = new Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
					this.PrintComplexGeometry(port, drawingContext, pageNumber, clipRect);
				}
				else
				{
					this.PrintSimplyGeometry(port, drawingContext, pageNumber, false);
				}
#endif
			}
		}

		// Calcule les zones d'impression.
		// Les différentes zones n'ont aucune intersection entre elles.
		protected System.Collections.ArrayList ComputeAreas(int pageNumber)
		{
			System.Collections.ArrayList areas = new System.Collections.ArrayList();
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( !this.PrintInfo.PerfectJoin && !obj.IsComplexPrinting )  continue;
					int i = PrintingArea.Intersect(areas, obj.BoundingBox);
					if ( i == -1 )
					{
						PrintingArea area = new PrintingArea(obj);
						areas.Add(area);
					}
					else
					{
						PrintingArea area = areas[i] as PrintingArea;
						area.Append(obj);
					}
				}
			}

			for ( int i=0 ; i<areas.Count ; i++ )
			{
				PrintingArea area1 = areas[i] as PrintingArea;

				bool merge;
				do
				{
					merge = false;
					for ( int j=i+1 ; j<areas.Count ; j++ )
					{
						PrintingArea area2 = areas[j] as PrintingArea;
						if ( area1.Area.IntersectsWith(area2.Area) )
						{
							area1.Append(area2);
							areas.RemoveAt(j);
							merge = true;
							break;
						}
					}
				}
				while ( merge );
			}

			return areas;
		}

		// PrintingArea représente une zone rectangulaire contenant un ou plusieurs
		// objets. Si un seul objet de la zone nécessite le mode complexe, toute la
		// zone est considérée comme complexe.
		protected class PrintingArea
		{
			public PrintingArea(Objects.Abstract obj)
			{
				this.area = obj.BoundingBox;
				this.isComplex = obj.IsComplexPrinting;
			}

			public void Append(Objects.Abstract obj)
			{
				this.area.MergeWith(obj.BoundingBox);
				this.isComplex |= obj.IsComplexPrinting;
			}

			public void Append(PrintingArea area)
			{
				this.area.MergeWith(area.Area);
				this.isComplex |= area.IsComplex;
			}

			public Rectangle Area
			{
				get { return this.area; }
			}

			public bool IsComplex
			{
				get { return this.isComplex; }
			}

			public static int Intersect(System.Collections.ArrayList areas, Rectangle bbox)
			{
				for ( int i=0 ; i<areas.Count ; i++ )
				{
					PrintingArea area = areas[i] as PrintingArea;
					if ( bbox.IntersectsWith(area.Area) )  return i;
				}
				return -1;
			}

			protected Rectangle		area;
			protected bool			isComplex;
		}

		// Indique si une impression complexe est nécessaire.
		protected bool IsComplexPrinting(int pageNumber)
		{
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( obj.IsComplexPrinting )  return true;
				}
			}
			return false;
		}

		// Imprime la géométrie simple de tous les objets, possible lorsque les
		// objets n'utilisent ni les dégradés ni la transparence.
		protected void PrintSimplyGeometry(Printing.PrintPort port,
										   DrawingContext drawingContext,
										   int pageNumber,
										   bool onlySimply)
		{
			Transform initialTransform = port.Transform;

			double zoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				Size paperSize = port.PageSettings.PaperSize.Size;
				double pw = paperSize.Width;
				double ph = paperSize.Height;
				if ( port.PageSettings.Landscape )
				{
					Misc.Swap(ref pw, ref ph);
				}
				double zoomH = pw / this.document.Size.Width;
				double zoomV = ph / this.document.Size.Height;
				zoom = System.Math.Min(zoomH, zoomV);
			}
			else
			{
				zoom = 0.1*this.PrintInfo.Zoom;
			}
			port.ScaleTransform(zoom, zoom, 0, 0);
			
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				Properties.ModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( onlySimply && obj.IsComplexPrinting )  continue;
					obj.PrintGeometry(port, drawingContext);
				}
			}

			port.Transform = initialTransform;
		}

		// Imprime la géométrie complexe de tous les objets, en utilisant
		// un bitmap intermédiaire.
		protected void PrintComplexGeometry(Printing.PrintPort port,
											DrawingContext drawingContext,
											int pageNumber,
											Rectangle clipRect)
		{
			Transform initialTransform = port.Transform;

			double dpi = this.PrintInfo.Dpi;

			double autoZoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				Size paperSize = port.PageSettings.PaperSize.Size;
				double pw = paperSize.Width*10.0;
				double ph = paperSize.Height*10.0;
				if ( port.PageSettings.Landscape )
				{
					Misc.Swap(ref pw, ref ph);
				}
				double zoomH = pw / this.document.Size.Width;
				double zoomV = ph / this.document.Size.Height;
				autoZoom = System.Math.Min(zoomH, zoomV);
			}

			Graphics gfx = new Graphics();
			int dx = (int) ((clipRect.Width/10.0)*(dpi/25.4)*autoZoom);
			int dy = (int) ((clipRect.Height/10.0)*(dpi/25.4)*autoZoom);
			gfx.SetPixmapSize(dx, dy);
			gfx.SolidRenderer.ClearARGB(1,1,1,1);
			gfx.Rasterizer.Gamma = this.PrintInfo.Gamma;

			double zoom = dx/clipRect.Width;
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(zoom, -zoom, 0, 0);
			gfx.TranslateTransform(-clipRect.Left, -clipRect.Bottom);

			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				Properties.ModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( !clipRect.IntersectsWith(obj.BoundingBox) )  continue;
					obj.DrawGeometry(gfx, drawingContext);
				}
			}

			port.ScaleTransform(0.1, 0.1, 0, 0);
			Image bitmap = Bitmap.FromPixmap(gfx.Pixmap);
			Rectangle rect = clipRect;
			rect.Scale(autoZoom);
			port.PaintImage(bitmap, rect);

			port.Transform = initialTransform;
		}

		// Imprime les zones rectangulaires (pour le debug).
		protected void PrintAreas(Printing.PrintPort port,
								  DrawingContext drawingContext,
								  System.Collections.ArrayList areas)
		{
			Transform initialTransform = port.Transform;

			double zoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				Size paperSize = port.PageSettings.PaperSize.Size;
				double pw = paperSize.Width;
				double ph = paperSize.Height;
				if ( port.PageSettings.Landscape )
				{
					Misc.Swap(ref pw, ref ph);
				}
				double zoomH = pw / this.document.Size.Width;
				double zoomV = ph / this.document.Size.Height;
				zoom = System.Math.Min(zoomH, zoomV);
			}
			else
			{
				zoom = 0.1*this.PrintInfo.Zoom;
			}
			port.ScaleTransform(zoom, zoom, 0, 0);
			
			port.LineWidth = 0.1;
			foreach ( PrintingArea area in areas )
			{
				Path path = new Path();
				path.AppendRectangle(area.Area);
				port.PaintOutline(path);
			}

			port.Transform = initialTransform;
		}

		// Exporte la géométrie complexe de tous les objets, en utilisant
		// un bitmap intermédiaire.
		protected string ExportGeometry(DrawingContext drawingContext, string filename)
		{
			ImageFormat format = this.imageFormat;
			double dpi = this.imageDpi;
			int depth = this.imageDepth;  // 8, 16, 24 ou 32
			int quality = (int) (this.imageQuality*100.0);  // 0..100
			ImageCompression compression = this.imageCompression;

			if ( format == ImageFormat.Unknown )
			{
				return "Format d'image inconnu";
			}

			Graphics gfx = new Graphics();
			int dx = (int) ((this.document.Size.Width/10.0)*(dpi/25.4));
			int dy = (int) ((this.document.Size.Height/10.0)*(dpi/25.4));
			gfx.SetPixmapSize(dx, dy);
			gfx.SolidRenderer.ClearARGB((depth==32)?0:1, 1,1,1);
			gfx.Rasterizer.Gamma = this.imageAA;

			double zoomH = dx / this.document.Size.Width;
			double zoomV = dy / this.document.Size.Height;
			double zoom = System.Math.Min(zoomH, zoomV);
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(zoom, -zoom, 0, 0);

			DrawingContext cView = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract activLayer = cView.RootObject(2);
			Objects.Abstract page = cView.RootObject(1);
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				drawingContext.IsDimmed = false;
				if ( layer != activLayer )  // calque passif ?
				{
					if ( layer.Type == Objects.LayerType.Hide )  continue;
					drawingContext.IsDimmed = (layer.Type == Objects.LayerType.Dimmed);
				}

				Properties.ModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					obj.DrawGeometry(gfx, drawingContext);
				}
			}

			Bitmap bitmap = Bitmap.FromPixmap(gfx.Pixmap) as Bitmap;
			if ( bitmap == null )  return "Pas de bitmap";

			try
			{
				byte[] data;
				System.IO.FileStream stream;
				data = bitmap.Save(format, depth, quality, compression);

				if ( System.IO.File.Exists(filename) )
				{
					System.IO.File.Delete(filename);
				}

				stream = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);
				stream.Write(data, 0, data.Length);
				stream.Close();
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			return "";  // ok
		}


		// Donne les réglages de l'impression.
		protected Settings.PrintInfo PrintInfo
		{
			get
			{
				return this.document.Settings.PrintInfo;
			}
		}


		#region SettingsMemory
		protected class SettingsMemory
		{
			public SettingsMemory(Document document)
			{
				this.document = document;

				this.duplex = DuplexMode.Simplex;
				this.collate = false;
				this.copies = 1;
				this.fromPage = 1;
				this.toPage = this.document.Modifier.StatisticTotalPages();
				this.printRange = PrintRange.AllPages;
				this.outputFileName = "";
				this.printToFile = false;
			}

			public void Save(PrinterSettings settings)
			{
				if ( settings.PrintRange == PrintRange.AllPages )
				{
					settings.FromPage = 1;
					settings.ToPage = this.document.Modifier.StatisticTotalPages();
				}

				if ( settings.PrintRange == PrintRange.SelectedPages )
				{
					settings.FromPage = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage+1;
					settings.ToPage = settings.FromPage;
				}

				this.duplex = settings.Duplex;
				this.collate = settings.Collate;
				this.copies = settings.Copies;
				this.fromPage = settings.FromPage;
				this.toPage = settings.ToPage;
				this.printRange = settings.PrintRange;
				this.outputFileName = settings.OutputFileName;
				this.printToFile = settings.PrintToFile;
			}

			public void Restore(PrinterSettings settings)
			{
				settings.Duplex = this.duplex;
				settings.Collate = this.collate;
				settings.Copies = this.copies;
				settings.FromPage = this.fromPage;
				settings.ToPage = this.toPage;
				settings.PrintRange = this.printRange;
				settings.OutputFileName = this.outputFileName;
				settings.PrintToFile = this.printToFile;

				settings.MinimumPage = 1;
				settings.MaximumPage = this.document.Modifier.StatisticTotalPages();

				if ( settings.ToPage > settings.MaximumPage )
				{
					settings.ToPage = settings.MaximumPage;
				}
			}

			protected Document					document;
			protected DuplexMode				duplex;
			protected bool						collate;
			protected int						copies;
			protected int						fromPage;
			protected int						toPage;
			protected PrintRange				printRange;
			protected string					outputFileName;
			protected bool						printToFile;
		}
		#endregion


		protected Document					document;
		protected ImageFormat				imageFormat;
		protected double					imageDpi;
		protected ImageCompression			imageCompression;
		protected int						imageDepth;
		protected double					imageQuality;
		protected double					imageAA;
		protected SettingsMemory			settingsMemory;
	}
}
