using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Printer implémente l'impression d'un document.
	/// </summary>
	/// 
	public class Printer
	{
		public Printer(Document document)
		{
			this.document = document;

			this.imageFormat = ImageFormat.Unknown;
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

			Point offset = new Point(0, 0);
			if ( !this.PrintInfo.AutoZoom )
			{
				Size paperSize = port.PageSettings.PaperSize.Size;
				double pw = paperSize.Width*10.0;
				double ph = paperSize.Height*10.0;
				if ( port.PageSettings.Landscape )
				{
					Misc.Swap(ref pw, ref ph);
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.BottomLeft ||
					 this.PrintInfo.Centring == Settings.PrintCentring.MiddleLeft ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopLeft    )
				{
					offset.X = this.PrintInfo.Margins;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.BottomCenter ||
					 this.PrintInfo.Centring == Settings.PrintCentring.MiddleCenter ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopCenter    )
				{
					offset.X = (pw-this.document.Size.Width)/2.0;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.BottomRight ||
					 this.PrintInfo.Centring == Settings.PrintCentring.MiddleRight ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopRight    )
				{
					offset.X = pw-this.document.Size.Width-this.PrintInfo.Margins;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.BottomLeft   ||
					 this.PrintInfo.Centring == Settings.PrintCentring.BottomCenter ||
					 this.PrintInfo.Centring == Settings.PrintCentring.BottomRight  )
				{
					offset.Y = this.PrintInfo.Margins;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.MiddleLeft   ||
					 this.PrintInfo.Centring == Settings.PrintCentring.MiddleCenter ||
					 this.PrintInfo.Centring == Settings.PrintCentring.MiddleRight  )
				{
					offset.Y = (ph-this.document.Size.Height)/2.0;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.TopLeft   ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopCenter ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopRight  )
				{
					offset.Y = ph-this.document.Size.Height-this.PrintInfo.Margins;
				}
			}

			if ( this.PrintInfo.ForceSimply )
			{
				this.PrintSimplyGeometry(port, drawingContext, pageNumber, offset);
			}
			else if ( this.PrintInfo.ForceComplex )
			{
				Rectangle clipRect = new Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
				this.PrintBitmapGeometry(port, drawingContext, pageNumber, offset, clipRect, null);
			}
			else
			{
				System.Collections.ArrayList areas = this.ComputeAreas(pageNumber);
				this.PrintMixGeometry(port, drawingContext, pageNumber, offset, areas);

				if ( this.PrintInfo.DebugArea )
				{
					this.PrintAreas(port, drawingContext, areas, offset);
				}
			}

			if ( this.PrintInfo.Target )
			{
				this.PrintTarget(port, drawingContext, offset);
			}

			if ( this.document.InstallType == InstallType.Demo )
			{
				this.PrintDemo(port, offset);
			}
			if ( this.document.InstallType == InstallType.Expired )
			{
				this.PrintExpired(port, offset);
			}
		}

		// Calcule les zones d'impression.
		// Les différentes zones n'ont aucune intersection entre elles.
		protected System.Collections.ArrayList ComputeAreas(int pageNumber)
		{
			System.Collections.ArrayList areas = new System.Collections.ArrayList();
			int rank = 0;
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( !this.PrintInfo.PerfectJoin && !obj.IsComplexPrinting )  continue;
					rank ++;

					int i = PrintingArea.Intersect(areas, obj.BoundingBox);
					if ( i == -1 )
					{
						PrintingArea area = new PrintingArea(obj, rank);
						areas.Add(area);
					}
					else
					{
						PrintingArea area = areas[i] as PrintingArea;
						area.Append(obj, rank);
					}
				}
			}

			// Fusionne toutes les zones qui se chevauchent.
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

			// Supprime toutes les zones ne contenant que des objets simples.
			if ( this.PrintInfo.PerfectJoin )
			{
				for ( int i=0 ; i<areas.Count ; i++ )
				{
					PrintingArea area = areas[i] as PrintingArea;
					if ( !area.IsComplex )
					{
						areas.RemoveAt(i);
						i --;
					}
				}
			}

			return areas;
		}

		// PrintingArea représente une zone rectangulaire contenant un ou plusieurs
		// objets complexes.
		protected class PrintingArea
		{
			public PrintingArea(Objects.Abstract obj, int rank)
			{
				this.area = obj.BoundingBox;
				this.isComplex = obj.IsComplexPrinting;
				this.topObject = obj;
				this.topRank = rank;
			}

			public void Append(Objects.Abstract obj, int rank)
			{
				this.area.MergeWith(obj.BoundingBox);
				this.isComplex |= obj.IsComplexPrinting;

				if ( rank > this.topRank )
				{
					this.topObject = obj;
					this.topRank = rank;
				}
			}

			public void Append(PrintingArea area)
			{
				this.area.MergeWith(area.area);
				this.isComplex |= area.isComplex;

				if ( area.topRank > this.topRank )
				{
					this.topObject = area.topObject;
					this.topRank = area.topRank;
				}
			}

			public Rectangle Area
			{
				get { return this.area; }
			}

			public bool IsComplex
			{
				get { return this.isComplex; }
			}

			public Objects.Abstract TopObject
			{
				get { return this.topObject; }
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

			public static int SearchTopObject(System.Collections.ArrayList areas, Objects.Abstract topObject)
			{
				for ( int i=0 ; i<areas.Count ; i++ )
				{
					PrintingArea area = areas[i] as PrintingArea;
					if ( area.topObject == topObject )  return i;
				}
				return -1;
			}

			protected Rectangle			area;
			protected bool				isComplex;
			protected Objects.Abstract	topObject;
			protected int				topRank;
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
										   Point offset)
		{
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, offset);

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
					obj.PrintGeometry(port, drawingContext);
				}
			}

			port.Transform = initialTransform;
		}

		// Imprime la géométrie composée d'objets simples et de zones complexes.
		protected void PrintMixGeometry(Printing.PrintPort port,
										DrawingContext drawingContext,
										int pageNumber,
										Point offset,
										System.Collections.ArrayList areas)
		{
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, offset);

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

					if ( this.PrintInfo.PerfectJoin )
					{
						int i = PrintingArea.SearchTopObject(areas, obj);
						if ( i != -1 )
						{
							PrintingArea area = areas[i] as PrintingArea;

							Transform saveTransform = port.Transform;
							port.Transform = initialTransform;
							this.PrintBitmapGeometry(port, drawingContext, pageNumber, offset, area.Area, area.TopObject);
							port.Transform = saveTransform;
						}
						else
						{
							obj.PrintGeometry(port, drawingContext);
						}
					}
					else
					{
						if ( obj.IsComplexPrinting )
						{
							int i = PrintingArea.SearchTopObject(areas, obj);
							if ( i == -1 )  continue;
							PrintingArea area = areas[i] as PrintingArea;

							Transform saveTransform = port.Transform;
							port.Transform = initialTransform;
							this.PrintBitmapGeometry(port, drawingContext, pageNumber, offset, area.Area, area.TopObject);
							port.Transform = saveTransform;
						}
						else
						{
							obj.PrintGeometry(port, drawingContext);
						}
					}
				}
			}

			port.Transform = initialTransform;
		}

		// Imprime la géométrie complexe de tous les objets, en utilisant
		// un bitmap intermédiaire.
		protected void PrintBitmapGeometry(Printing.PrintPort port,
										   DrawingContext drawingContext,
										   int pageNumber,
										   Point offset,
										   Rectangle clipRect,
										   Objects.Abstract topObject)
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

			double zoomX = dx/clipRect.Width;
			double zoomY = dy/clipRect.Height;
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(zoomX, -zoomY, 0, 0);
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
					if ( obj == topObject )  goto stop;
				}
			}

			stop:
			port.ScaleTransform(0.1, 0.1, 0, 0);
			Image bitmap = Bitmap.FromPixmap(gfx.Pixmap);
			Rectangle rect = clipRect;
			rect.Offset(offset);
			rect.Scale(autoZoom);
			port.PaintImage(bitmap, rect);

			port.Transform = initialTransform;
		}

		// Imprime les zones rectangulaires (pour le debug).
		protected void PrintAreas(Printing.PrintPort port,
								  DrawingContext drawingContext,
								  System.Collections.ArrayList areas,
								  Point offset)
		{
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, offset);

			port.LineWidth = 0.1;
			port.Color = Color.FromRGB(1,0,0);  // rouge

			foreach ( PrintingArea area in areas )
			{
				Path path = new Path();
				path.AppendRectangle(area.Area);
				port.PaintOutline(path);
			}

			port.Transform = initialTransform;
		}

		// Imprime les traits de coupe.
		protected void PrintTarget(Printing.PrintPort port,
								   DrawingContext drawingContext,
								   Point offset)
		{
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, offset);
			this.PaintTarget(port, drawingContext);
			port.Transform = initialTransform;
		}

		// Dessine les traits de coupe.
		public void PaintTarget(Drawing.IPaintPort port, DrawingContext drawingContext)
		{
			if ( port is Printing.PrintPort )
			{
				port.LineWidth = 0.1;
			}
			else
			{
				port.LineWidth = 1.0/drawingContext.ScaleX;
			}

			port.Color = Color.FromBrightness(0);  // noir

			Size ds = this.document.Size;
			double db = this.PrintInfo.Debord;
			double len = 50.0;  // longueur des traits de coupe = 5mm

			Path path = new Path();
			path.MoveTo(-db, 0);  path.LineTo(-db-len, 0);
			path.MoveTo(-db, ds.Height);  path.LineTo(-db-len, ds.Height);
			path.MoveTo(ds.Width+db, 0);  path.LineTo(ds.Width+db+len, 0);
			path.MoveTo(ds.Width+db, ds.Height);  path.LineTo(ds.Width+db+len, ds.Height);
			path.MoveTo(0, -db);  path.LineTo(0, -db-len);
			path.MoveTo(ds.Width, -db);  path.LineTo(ds.Width, -db-len);
			path.MoveTo(0, ds.Height+db);  path.LineTo(0, ds.Height+db+len);
			path.MoveTo(ds.Width, ds.Height+db);  path.LineTo(ds.Width, ds.Height+db+len);
			port.PaintOutline(path);

			len = 60.0;  // échantillons carrés de 6mm de côté
			Drawing.Rectangle rect = new Rectangle();
			rect.Left = len;
			rect.Bottom = ds.Height+db+len/2.0;
			rect.Width = len;
			rect.Height = len;

			if ( port is Graphics )
			{
				Graphics graphics = port as Graphics;
				graphics.Align(ref rect);
				rect.Offset(0.5/drawingContext.ScaleX, 0.5/drawingContext.ScaleX);
			}

			this.PaintColorSample(port, rect, Color.FromBrightness(1.0));  // blanc
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.9));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.8));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.7));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.6));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.5));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.4));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.3));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.2));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.1));
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromBrightness(0.0));  // noir
			rect.Offset(rect.Width+len/2.0, 0.0);
			this.PaintColorSample(port, rect, Color.FromRGB(0,1,1));  // cyan
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromRGB(1,0,1));  // magenta
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromRGB(1,1,0));  // jaune
		}

		// Dessine un échantillon de couleur.
		protected void PaintColorSample(Drawing.IPaintPort port, Rectangle rect, Color color)
		{
			Path path = new Path();
			path.AppendRectangle(rect);

			port.Color = color;
			port.PaintSurface(path);

			port.Color = Color.FromBrightness(0);
			port.PaintOutline(path);
		}

		// Imprime le warning d'installation.
		protected void PrintDemo(Printing.PrintPort port, Point offset)
		{
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, offset);
			this.PaintDemo(port);
			port.Transform = initialTransform;
		}

		// Desine le warning d'installation.
		protected void PaintDemo(Drawing.IPaintPort port)
		{
			Size ds = this.document.Size;
			Path path = new Path();

			path.MoveTo(Printer.Conv(ds,  2,4));
			path.LineTo(Printer.Conv(ds,  2,0));
			path.LineTo(Printer.Conv(ds,  0,0));
			path.LineTo(Printer.Conv(ds,  0,2));
			path.LineTo(Printer.Conv(ds,  2,2));  // d

			path.MoveTo(Printer.Conv(ds,  5,2));
			path.LineTo(Printer.Conv(ds,  3,2));
			path.LineTo(Printer.Conv(ds,  3,0));
			path.LineTo(Printer.Conv(ds,  5,0));
			path.MoveTo(Printer.Conv(ds,  3,1));
			path.LineTo(Printer.Conv(ds,  4,1));  // e

			path.MoveTo(Printer.Conv(ds,  6,0));
			path.LineTo(Printer.Conv(ds,  6,2));
			path.LineTo(Printer.Conv(ds,  8,2));
			path.LineTo(Printer.Conv(ds,  8,0));
			path.MoveTo(Printer.Conv(ds,  7,0));
			path.LineTo(Printer.Conv(ds,  7,2));  // m
			
			path.MoveTo(Printer.Conv(ds,  9,0));
			path.LineTo(Printer.Conv(ds,  9,2));
			path.LineTo(Printer.Conv(ds, 11,2));
			path.LineTo(Printer.Conv(ds, 11,0));
			path.LineTo(Printer.Conv(ds,  9,0));  // o
			
			port.Color = Color.FromBrightness(1.0);
			port.LineWidth = 20.0;
			port.PaintOutline(path);

			port.Color = Color.FromBrightness(0.8);
			port.LineWidth = 10.0;
			port.PaintOutline(path);
		}

		// Imprime le warning d'installation.
		protected void PrintExpired(Printing.PrintPort port, Point offset)
		{
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, offset);
			this.PaintExpired(port);
			port.Transform = initialTransform;
		}

		// Dessine le warning d'installation.
		protected void PaintExpired(Drawing.IPaintPort port)
		{
			Size ds = this.document.Size;
			Path path = new Path();

			path.MoveTo(Printer.Conv(ds,  2,2));
			path.LineTo(Printer.Conv(ds,  0,2));
			path.LineTo(Printer.Conv(ds,  0,0));
			path.LineTo(Printer.Conv(ds,  2,0));
			path.MoveTo(Printer.Conv(ds,  0,1));
			path.LineTo(Printer.Conv(ds,  1,1));  // e

			path.MoveTo(Printer.Conv(ds,  5,2));
			path.LineTo(Printer.Conv(ds,  3,2));
			path.LineTo(Printer.Conv(ds,  3,0));
			path.LineTo(Printer.Conv(ds,  5,0));  // c

			path.MoveTo(Printer.Conv(ds,  6,4));
			path.LineTo(Printer.Conv(ds,  6,0));
			path.MoveTo(Printer.Conv(ds,  6,2));
			path.LineTo(Printer.Conv(ds,  8,2));
			path.LineTo(Printer.Conv(ds,  8,0));  // h
			
			path.MoveTo(Printer.Conv(ds,  9,2));
			path.LineTo(Printer.Conv(ds,  9,0));
			path.LineTo(Printer.Conv(ds, 11,0));
			path.LineTo(Printer.Conv(ds, 11,2));  // u
			
			port.Color = Color.FromBrightness(1.0);
			port.LineWidth = 20.0;
			port.PaintOutline(path);

			port.Color = Color.FromBrightness(0.8);
			port.LineWidth = 10.0;
			port.PaintOutline(path);
		}

		protected static Point Conv(Size size, double x, double y)
		{
			double margin = System.Math.Min(size.Width, size.Height)/4.0;

			if ( size.Width > size.Height )
			{
				double xx = margin + (size.Width-margin*2.0)*(x/11.0);
				double yy = margin + (size.Height-margin*2.0)*(y/4.0);
				return new Point(xx, yy);
			}
			else
			{
				double xx = size.Width - margin - (size.Width-margin*2.0)*(y/4.0);
				double yy = margin + (size.Height-margin*2.0)*(x/11.0);
				return new Point(xx, yy);
			}
		}

		// Initialise le port pour une impression simplifiée.
		protected void InitSimplyPort(Printing.PrintPort port, Point offset)
		{
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
			port.TranslateTransform(offset.X, offset.Y);
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

			if ( this.document.InstallType == InstallType.Demo )
			{
				this.PaintDemo(gfx);
			}
			if ( this.document.InstallType == InstallType.Expired )
			{
				this.PaintExpired(gfx);
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
