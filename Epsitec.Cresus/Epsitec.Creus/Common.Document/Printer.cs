using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

using System.Collections.Generic;

namespace Epsitec.Common.Document
{
	public enum ExportImageCrop
	{
		Page      = 0,		// cadrer sur la page
		Objects   = 1,		// cadrer sur les objets
		Selection = 2,		// cadrer sur les objets sélectionnés
	}


	/// <summary>
	/// La classe Printer implémente l'impression d'un document.
	/// </summary>
	/// 
	public class Printer
	{
		public Printer(Document document)
		{
			this.document = document;

			this.imageCrop = ExportImageCrop.Page;
			this.imageOnlySelected = false;
			this.imageFormat = ImageFormat.Unknown;
			this.imageDpi = (this.document.Type == DocumentType.Pictogram) ? 254 : 100;
			this.imageCompression = ImageCompression.None;
			this.imageDepth = 24;
			this.imageQuality = 0.85;
			this.imageAA = 1.0;

			this.imageNameFilters = new string[2];
			this.imageNameFilters[0] = "Blackman";
			this.imageNameFilters[1] = "Bicubic";
		}

		public void Dispose()
		{
		}


		public void Print(Epsitec.Common.Dialogs.PrintDialog dp)
		{
			//	Imprime le document selon les choix faits dans le dialogue Window (dp)
			//	ainsi que dans le dialogue des réglages (PrintInfo).
			PrintEngine printEngine = new PrintEngine();
			if (printEngine.Initialize(this, dp))
			{
				dp.Document.Print(printEngine);
			}
		}

		public string Export(string filename)
		{
			//	Exporte le document dans un fichier bitmap.
			//	Crée le DrawingContext utilisé pour l'exportation.
			DrawingContext drawingContext = new DrawingContext(this.document, null);
			drawingContext.ContainerSize = this.document.PageSize;
			drawingContext.PreviewActive = true;
			drawingContext.IsBitmap = true;
			drawingContext.GridShow = false;
			drawingContext.SetImageNameFilter(0, this.document.Printer.GetImageNameFilter(0));  // filtre A
			drawingContext.SetImageNameFilter(1, this.document.Printer.GetImageNameFilter(1));  // filtre B

			return this.ExportGeometry(drawingContext, filename, this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage);
		}

        public string ExportICO(string filename)
        {
            //	Exporte le document dans une icône windows.
            //	Crée le DrawingContext utilisé pour l'exportation.
            DrawingContext drawingContext = new DrawingContext(this.document, null);
            drawingContext.ContainerSize = this.document.PageSize;
            drawingContext.PreviewActive = true;
            drawingContext.IsBitmap = true;
            drawingContext.GridShow = false;
            drawingContext.SetImageNameFilter(0, this.document.Printer.GetImageNameFilter(0));  // filtre A
            drawingContext.SetImageNameFilter(1, this.document.Printer.GetImageNameFilter(1));  // filtre B

            return this.ExportGeometryICO(drawingContext, filename, this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage);
        }

        public bool Miniature(Size sizeHope, bool isModel, out string filename, out byte[] data)
		{
			//	Retourne les données pour l'image bitmap miniature de la première page.
			DrawingContext drawingContext = new DrawingContext(this.document, null);
			drawingContext.ContainerSize = this.document.PageSize;
			drawingContext.PreviewActive = false;
			drawingContext.IsBitmap = true;
			drawingContext.GridShow = isModel;

			int pageNumber = this.document.Modifier.PrintablePageRank(0);  // numéro de la première page non modèle du document

			Size pageSize = this.document.GetPageSize(pageNumber);
			double dpix = sizeHope.Width*254/pageSize.Width;
			double dpiy = sizeHope.Height*254/pageSize.Height;
			double dpi = System.Math.Min(dpix, dpiy);
			
			string err = this.ExportGeometry(drawingContext, pageNumber, ImageFormat.Png, dpi, ImageCompression.None, 24, 85, 1, false, false, ExportImageCrop.Page, out data);
			if (err == "")
			{
				filename = "preview.png";
				return true;
			}
			else
			{
				filename = null;
				data = null;
				return false;
			}
		}

		public Bitmap CreateMiniatureBitmap(Size sizeHope, bool isModel, int page, int layer)
		{
			//	Retourne les données pour l'image bitmap d'une miniature.
			//	Si layer == -1, on dessine tous les calques.
			DrawingContext drawingContext = new DrawingContext(this.document, null);
			drawingContext.IsBitmap = true;
			drawingContext.ContainerSize = this.document.PageSize;
			drawingContext.PreviewActive = true;
			drawingContext.TextShowControlCharacters = false;
			drawingContext.GridShow = isModel;

			Size pageSize = this.document.GetPageSize(page);
			double dpix = sizeHope.Width*254/pageSize.Width;
			double dpiy = sizeHope.Height*254/pageSize.Height;
			double dpi = System.Math.Min(dpix, dpiy);

			return this.ExportBitmap(drawingContext, page, layer, dpi, 24, 1, false, false, ExportImageCrop.Page);
		}


		#region Image
		public static ImageFormat GetImageFormat(string ext)
		{
			//	Trouve le type d'une image en fonction de l'extension.
			switch ( ext.ToLower() )
			{
				case ".bmp":  return ImageFormat.Bmp;
				case ".gif":  return ImageFormat.Gif;
				case ".tif":  return ImageFormat.Tiff;
				case ".jpg":  return ImageFormat.Jpeg;
				case ".png":  return ImageFormat.Png;
				case ".exf":  return ImageFormat.Exif;
				case ".emf":  return ImageFormat.WindowsEmf;
				case ".wmf":  return ImageFormat.WindowsWmf;
			}
			return ImageFormat.Unknown;
		}

		public bool ImageOnlySelected
		{
			get
			{
				return this.document.Modifier.TotalSelected > 0 && this.imageOnlySelected;
			}
			set
			{
				this.imageOnlySelected = value;
			}
		}

		public ExportImageCrop ImageCrop
		{
			get
			{
				if (this.imageCrop == ExportImageCrop.Selection && this.document.Modifier.TotalSelected == 0)
				{
					return ExportImageCrop.Objects;
				}
				return this.imageCrop;
			}
			set
			{
				this.imageCrop = value;
			}
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

		public string GetImageNameFilter(int rank)
		{
			//	Donne le nom d'un filtre pour l'image.
			System.Diagnostics.Debug.Assert(rank >= 0 && rank < this.imageNameFilters.Length);
			return this.imageNameFilters[rank];
		}

		public void SetImageNameFilter(int rank, string name)
		{
			//	Modifie le nom d'un filtre pour l'image.
			System.Diagnostics.Debug.Assert(rank >= 0 && rank < this.imageNameFilters.Length);
			this.imageNameFilters[rank] = name;
		}

		public void SetImagePixelWidth(double width)
		{
			//	Adapte la résolution d'après une largeur en pixel souhaitée.
			Size size = this.ImageSize;
			this.imageDpi = (width*10.0*25.4)/size.Width;
		}

		public void SetImagePixelHeight(double height)
		{
			//	Adapte la résolution d'après une hauteur en pixel souhaitée.
			Size size = this.ImageSize;
			this.imageDpi = (height*10.0*25.4)/size.Height;
		}

		public Size ImagePixelSize
		{
			//	Retourne la taille en pixels de l'image exportée.
			get
			{
				Size size = this.ImageSize;

				size.Width  = (size.Width/10.0)*(this.imageDpi/25.4);
				size.Height = (size.Height/10.0)*(this.imageDpi/25.4);
				
				return size;
			}
		}

		protected Size ImageSize
		{
			//	Retourne la taille de l'image exportée.
			get
			{
				if (this.ImageCrop == ExportImageCrop.Page)
				{
					return this.document.PageSize;
				}
				else
				{
					int pageNumber = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
					return this.GetBoundingBox(pageNumber, this.ImageCrop == ExportImageCrop.Selection).Size;
				}
			}
		}
		#endregion


		protected class PrintEngine : Printing.IPrintEngine
		{
			public bool Initialize(Printer printer, Epsitec.Common.Dialogs.PrintDialog dp)
			{
				this.printer = printer;
				this.document = printer.document;

				this.pageList = new List<int>();
				this.pageCounter = 0;

				Settings.PrintInfo pi = this.document.Settings.PrintInfo;

				//	Crée le DrawingContext utilisé pour l'impression.
				this.drawingContext = new DrawingContext(this.document, null);
				this.drawingContext.ContainerSize = this.document.DocumentSize;
				this.drawingContext.PreviewActive = true;
				this.drawingContext.GridShow = false;
				this.drawingContext.SetImageNameFilter(0, pi.GetImageNameFilter(0));  // filtre A
				this.drawingContext.SetImageNameFilter(1, pi.GetImageNameFilter(1));  // filtre B

				if ( dp.Document.PrinterSettings.PrinterName != pi.PrintName )
				{
					dp.Document.SelectPrinter(pi.PrintName);
				}
				
				dp.Document.PrinterSettings.Collate = pi.Collate;
				dp.Document.PrinterSettings.PrintToFile = pi.PrintToFile;
				dp.Document.PrinterSettings.OutputFileName = pi.PrintFilename;

				Settings.PrintRange range = pi.PrintRange;
				int copies = pi.Copies;

				int fromPage = 1;
				int toPage   = this.document.Modifier.PrintableTotalPages();
				bool justeOneMaster = false;

				if ( range == Settings.PrintRange.FromTo )
				{
					fromPage = pi.PrintFrom;
					toPage   = pi.PrintTo;
				}
				else if ( range == Settings.PrintRange.Current )
				{
					int cp = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
					Objects.Page page = this.document.DocumentObjects[cp] as Objects.Page;
					if ( page.MasterType == Objects.MasterType.Slave )
					{
						fromPage = page.Rank+1;
						toPage   = fromPage;
					}
					else
					{
						fromPage = cp;
						toPage   = cp;
						justeOneMaster = true;
					}
				}

				bool reverse = false;
				if ( fromPage > toPage )
				{
					Misc.Swap(ref fromPage, ref toPage);
					reverse = true;
				}

				int totalPages = toPage-fromPage+1;

				//	Reprend ici tous les choix effectués dans le dialogue Window
				//	de l'impression. Même s'il semble possible de les atteindre
				//	plus tard avec port.PageSettings.PrinterSettings, cela
				//	fonctionne mal.
				this.paperSize = dp.Document.PrinterSettings.DefaultPageSettings.PaperSize.Size;
				this.landscape = dp.Document.PrinterSettings.DefaultPageSettings.Landscape;

				bool multicopyByPrinter = false;

				if ( pi.Collate == false &&
					 dp.Document.PrinterSettings.Duplex == DuplexMode.Simplex &&
					 pi.Copies > 1 )
				{
					multicopyByPrinter = true;
				}

				if ( multicopyByPrinter )
				{
					dp.Document.PrinterSettings.Copies = pi.Copies;
					copies = 1;
				}
				else
				{
					dp.Document.PrinterSettings.Copies = 1;
				}

				//	Calcule la liste des pages à imprimer.
				if ( justeOneMaster )
				{
					for ( int i=0 ; i<copies ; i++ )
					{
						this.pageList.Add(fromPage);
					}
				}
				else
				{
					for ( int i=0 ; i<totalPages*copies ; i++ )
					{
						int page = i;

						if ( pi.Collate )  // 1,2,3,4 - 1,2,3,4 ?
						{
							page %= totalPages;
						}
						else	// 1,1 - 2,2 - 3,3 - 4,4 ?
						{
							if ( dp.Document.PrinterSettings.Duplex != DuplexMode.Simplex )  // 1,2 - 1,2 - 3,4 - 3,4 ?
							{
								page = (page/(copies*2)*2)|(page&0x1);
							}
							else
							{
								page /= copies;
							}
						}
						page += fromPage-1;
						page = this.document.Modifier.PrintablePageRank(page);
						if ( page == -1 )  break;

						if ( pi.PrintArea == Settings.PrintArea.Even )
						{
							if ( page%2 == 0 )  continue;
						}
						if ( pi.PrintArea == Settings.PrintArea.Odd )
						{
							if ( page%2 != 0 )  continue;
						}

						this.pageList.Add(page);
					}
				}

				if ( pi.Reverse ^ reverse )
				{
					this.pageList.Reverse();
				}

				return this.pageList.Count > 0;
			}

			public Size PaperSize
			{
				get { return this.paperSize; }
			}

			public bool Landscape
			{
				get { return this.landscape; }
				set { this.landscape = value; }
			}

			#region IPrintEngine Members
			public void PrepareNewPage(Printing.PageSettings settings)
			{
				//?System.Diagnostics.Debug.WriteLine("PrepareNewPage");
				settings.Margins = new Margins(0, 0, 0, 0);

				if ( this.pageCounter < this.pageList.Count )
				{
					if ( this.printer.PrintInfo.AutoLandscape )
					{
						int page = this.pageList[this.pageCounter];
						Size pageSize = this.document.GetPageSize(page);
						settings.Landscape = (pageSize.Width > pageSize.Height);
					}
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
				if ( this.pageCounter >= this.pageList.Count )
				{
					return Printing.PrintEngineStatus.FinishJob;
				}

				int page = this.pageList[this.pageCounter++];
				//?System.Diagnostics.Debug.WriteLine("PrintPage "+page.ToString());
				this.printer.PrintGeometry(port, this, this.drawingContext, page);

				if ( this.pageCounter < this.pageList.Count )
				{
					return Printing.PrintEngineStatus.MorePages;
				}
				return Printing.PrintEngineStatus.FinishJob;
			}
			#endregion

			protected Document						document;
			protected Printer						printer;
			protected DrawingContext				drawingContext;
			protected Size							paperSize;
			protected bool							landscape;
			protected List<int>						pageList;
			protected int							pageCounter;
		}


		protected void PrintGeometry(Printing.PrintPort port,
									 PrintEngine printEngine,
									 DrawingContext drawingContext,
									 int pageNumber)
		{
			//	Imprime la géométrie de tous les objets.
			System.Diagnostics.Debug.Assert(pageNumber >= 0);
			System.Diagnostics.Debug.Assert(pageNumber < this.document.DocumentObjects.Count);

			printEngine.Landscape = port.PageSettings.Landscape;
			Size pageSize = this.document.GetPageSize(pageNumber);

			Point offset = new Point(0, 0);
			if ( !this.PrintInfo.AutoZoom )
			{
				double pw = printEngine.PaperSize.Width*10.0;
				double ph = printEngine.PaperSize.Height*10.0;
				if ( printEngine.Landscape )
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
					offset.X = (pw-pageSize.Width)/2.0;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.BottomRight ||
					 this.PrintInfo.Centring == Settings.PrintCentring.MiddleRight ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopRight    )
				{
					offset.X = pw-pageSize.Width-this.PrintInfo.Margins;
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
					offset.Y = (ph-pageSize.Height)/2.0;
				}

				if ( this.PrintInfo.Centring == Settings.PrintCentring.TopLeft   ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopCenter ||
					 this.PrintInfo.Centring == Settings.PrintCentring.TopRight  )
				{
					offset.Y = ph-pageSize.Height-this.PrintInfo.Margins;
				}
			}

			if ( this.PrintInfo.ForceSimply )
			{
				this.PrintSimplyGeometry(port, printEngine, drawingContext, pageNumber, offset);
			}
			else if ( this.PrintInfo.ForceComplex )
			{
				Rectangle clipRect = new Rectangle(0, 0, pageSize.Width, pageSize.Height);
				this.PrintBitmapGeometry(port, printEngine, drawingContext, pageNumber, offset, clipRect, null);
			}
			else
			{
				System.Collections.ArrayList areas = this.ComputeAreas(pageNumber);
				this.PrintMixGeometry(port, printEngine, drawingContext, pageNumber, offset, areas);

				if ( this.PrintInfo.DebugArea )
				{
					this.PrintAreas(port, printEngine, drawingContext, areas, offset, pageNumber);
				}
			}

			if ( this.PrintInfo.Target )
			{
				this.PrintTarget(port, printEngine, drawingContext, offset, pageNumber);
			}

			if ( this.document.InstallType == InstallType.Demo )
			{
				this.PrintDemo(port, printEngine, offset, pageNumber);
			}
			if ( this.document.InstallType == InstallType.Expired )
			{
				this.PrintExpired(port, printEngine, offset, pageNumber);
			}
		}

		protected System.Collections.ArrayList ComputeLayers(int pageNumber)
		{
			//	Calcule la liste des calques, y compris ceux des pages maîtres.
			//	Les calques cachés à l'impression ne sont pas mis dans la liste.
			System.Collections.ArrayList layers = new System.Collections.ArrayList();
			List<Objects.Page> masterList = this.document.Modifier.ComputeMasterPageList(pageNumber);

			//	Mets d'abord les premiers calques de toutes les pages maîtres.
			foreach ( Objects.Page master in masterList )
			{
				int frontier = master.MasterFirstFrontLayer;
				for ( int i=0 ; i<frontier ; i++ )
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;
					layers.Add(layer);
				}
			}

			//	Mets ensuite tous les calques de la page.
			Objects.Abstract page = this.document.DocumentObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;
				layers.Add(layer);
			}

			//	Mets finalement les derniers calques de toutes les pages maîtres.
			foreach ( Objects.Page master in masterList )
			{
				int frontier = master.MasterFirstFrontLayer;
				int total = master.Objects.Count;
				for ( int i=frontier ; i<total ; i++ )
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;
					layers.Add(layer);
				}
			}

			return layers;
		}

		protected System.Collections.ArrayList ComputeAreas(int pageNumber)
		{
			//	Calcule les zones d'impression.
			//	Les différentes zones n'ont aucune intersection entre elles.
			System.Collections.ArrayList areas = new System.Collections.ArrayList();
			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			int rank = 0;
			foreach ( Objects.Layer layer in layers )
			{
				Properties.ModColor modColor = layer.PropertyModColor;
				bool isLayerComplexPrinting = modColor.IsComplexPrinting;

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( !this.PrintInfo.PerfectJoin && !isLayerComplexPrinting && !obj.IsComplexPrinting )  continue;
					rank ++;

					int i = PrintingArea.Intersect(areas, obj.BoundingBox);
					if ( i == -1 )
					{
						PrintingArea area = new PrintingArea(obj, rank, isLayerComplexPrinting);
						areas.Add(area);
					}
					else
					{
						PrintingArea area = areas[i] as PrintingArea;
						area.Append(obj, rank, isLayerComplexPrinting);
					}
				}
			}

			//	Fusionne toutes les zones qui se chevauchent.
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

			//	Supprime toutes les zones ne contenant que des objets simples.
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

		//	PrintingArea représente une zone rectangulaire contenant un ou plusieurs
		//	objets complexes.
		protected class PrintingArea
		{
			public PrintingArea(Objects.Abstract obj, int rank, bool isLayerComplexPrinting)
			{
				this.area = obj.BoundingBox;
				this.isComplex = (isLayerComplexPrinting || obj.IsComplexPrinting);
				this.topObject = obj;
				this.topRank = rank;
			}

			public void Append(Objects.Abstract obj, int rank, bool isLayerComplexPrinting)
			{
				this.area.MergeWith(obj.BoundingBox);
				this.isComplex |= (isLayerComplexPrinting || obj.IsComplexPrinting);

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

		protected bool IsComplexPrinting(int pageNumber)
		{
			//	Indique si une impression complexe est nécessaire.
			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			foreach ( Objects.Layer layer in layers )
			{
				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( obj.IsComplexPrinting )  return true;
				}
			}
			return false;
		}

		protected void PrintSimplyGeometry(Printing.PrintPort port,
										   PrintEngine printEngine,
										   DrawingContext drawingContext,
										   int pageNumber,
										   Point offset)
		{
			//	Imprime la géométrie simple de tous les objets, possible lorsque les
			//	objets n'utilisent ni les dégradés ni la transparence.
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, printEngine, offset, pageNumber);

			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			foreach ( Objects.Layer layer in layers )
			{
				Properties.ModColor modColor = layer.PropertyModColor;
				port.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				port.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					obj.DrawGeometry(port, drawingContext);
				}

				port.PopColorModifier();
				port.PopColorModifier();
			}

			port.Transform = initialTransform;
		}

		protected void PrintMixGeometry(Printing.PrintPort port,
										PrintEngine printEngine,
										DrawingContext drawingContext,
										int pageNumber,
										Point offset,
										System.Collections.ArrayList areas)
		{
			//	Imprime la géométrie composée d'objets simples et de zones complexes.
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, printEngine, offset, pageNumber);

			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			foreach ( Objects.Layer layer in layers )
			{
				Properties.ModColor modColor = layer.PropertyModColor;
				port.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				port.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));
				bool isLayerComplexPrinting = modColor.IsComplexPrinting;

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
							this.PrintBitmapGeometry(port, printEngine, drawingContext, pageNumber, offset, area.Area, area.TopObject);
							port.Transform = saveTransform;
						}
						else
						{
							obj.DrawGeometry(port, drawingContext);
						}
					}
					else
					{
						if ( isLayerComplexPrinting || obj.IsComplexPrinting )
						{
							int i = PrintingArea.SearchTopObject(areas, obj);
							if ( i == -1 )  continue;
							PrintingArea area = areas[i] as PrintingArea;

							Transform saveTransform = port.Transform;
							port.Transform = initialTransform;
							this.PrintBitmapGeometry(port, printEngine, drawingContext, pageNumber, offset, area.Area, area.TopObject);
							port.Transform = saveTransform;
						}
						else
						{
							obj.DrawGeometry(port, drawingContext);
						}
					}
				}

				port.PopColorModifier();
				port.PopColorModifier();
			}

			port.Transform = initialTransform;
		}

		protected void PrintBitmapGeometry(Printing.PrintPort port,
										   PrintEngine printEngine,
										   DrawingContext drawingContext,
										   int pageNumber,
										   Point offset,
										   Rectangle clipRect,
										   Objects.Abstract topObject)
		{
			//	Imprime la géométrie complexe de tous les objets, en utilisant
			//	un bitmap intermédiaire.
			Transform initialTransform = port.Transform;

			double dpi = this.PrintInfo.Dpi;

			double autoZoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				double pw = printEngine.PaperSize.Width*10.0;
				double ph = printEngine.PaperSize.Height*10.0;
				if ( printEngine.Landscape )
				{
					Misc.Swap(ref pw, ref ph);
				}
				double zoomH = pw / this.document.GetPageSize(pageNumber).Width;
				double zoomV = ph / this.document.GetPageSize(pageNumber).Height;
				autoZoom = System.Math.Min(zoomH, zoomV);
			}

			Graphics gfx = new Graphics();
			int dx = (int) ((clipRect.Width/10.0)*(dpi/25.4)*autoZoom);
			int dy = (int) ((clipRect.Height/10.0)*(dpi/25.4)*autoZoom);
			gfx.SetPixmapSize(dx, dy);
			gfx.SolidRenderer.ClearAlphaRgb(1,1,1,1);
			gfx.Rasterizer.Gamma = this.PrintInfo.Gamma;

			double zoomX = dx/clipRect.Width;
			double zoomY = dy/clipRect.Height;
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(zoomX, -zoomY, 0, 0);
			gfx.TranslateTransform(-clipRect.Left, -clipRect.Bottom);

			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			foreach ( Objects.Layer layer in layers )
			{
				Properties.ModColor modColor = layer.PropertyModColor;
				port.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				port.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( !clipRect.IntersectsWith(obj.BoundingBox) )  continue;
					obj.DrawGeometry(gfx, drawingContext);
					if ( obj == topObject )  goto stop;
				}

				port.PopColorModifier();
				port.PopColorModifier();
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

		protected void PrintAreas(Printing.PrintPort port,
								  PrintEngine printEngine,
								  DrawingContext drawingContext,
								  System.Collections.ArrayList areas,
								  Point offset,
								  int pageNumber)
		{
			//	Imprime les zones rectangulaires (pour le debug).
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, printEngine, offset, pageNumber);

			port.LineWidth = 0.1;
			port.Color = Color.FromRgb(1,0,0);  // rouge

			foreach ( PrintingArea area in areas )
			{
				Path path = new Path();
				path.AppendRectangle(area.Area);
				port.PaintOutline(path);
			}

			port.Transform = initialTransform;
		}

		protected void PrintTarget(Printing.PrintPort port,
								   PrintEngine printEngine,
								   DrawingContext drawingContext,
								   Point offset,
								   int pageNumber)
		{
			//	Imprime les traits de coupe.
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, printEngine, offset, pageNumber);
			this.PaintTarget(port, drawingContext, pageNumber);
			port.Transform = initialTransform;
		}

		public void PaintTarget(Drawing.IPaintPort port, DrawingContext drawingContext, int pageNumber)
		{
			//	Dessine les traits de coupe.
			if ( port is Printing.PrintPort )
			{
				port.LineWidth = 0.1;
			}
			else
			{
				port.LineWidth = 1.0/drawingContext.ScaleX;
			}

			port.Color = Color.FromBrightness(0);  // noir

			Size ds = this.document.GetPageSize(pageNumber);
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
			this.PaintColorSample(port, rect, Color.FromRgb(0,1,1));  // cyan
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromRgb(1,0,1));  // magenta
			rect.Offset(rect.Width, 0.0);
			this.PaintColorSample(port, rect, Color.FromRgb(1,1,0));  // jaune
		}

		protected void PaintColorSample(Drawing.IPaintPort port, Rectangle rect, Color color)
		{
			//	Dessine un échantillon de couleur.
			Path path = new Path();
			path.AppendRectangle(rect);

			port.Color = color;
			port.PaintSurface(path);

			port.Color = Color.FromBrightness(0);
			port.PaintOutline(path);
		}

		protected void PrintDemo(Printing.PrintPort port, PrintEngine printEngine, Point offset, int pageNumber)
		{
			//	Imprime le warning d'installation.
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, printEngine, offset, pageNumber);
			this.PaintDemo(port, pageNumber);
			port.Transform = initialTransform;
		}

		protected void PaintDemo(Drawing.IPaintPort port, int pageNumber)
		{
			//	Desine le warning d'installation.
			Size ds = this.document.GetPageSize(pageNumber);
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

		protected void PrintExpired(Printing.PrintPort port, PrintEngine printEngine, Point offset, int pageNumber)
		{
			//	Imprime le warning d'installation.
			Transform initialTransform = port.Transform;
			this.InitSimplyPort(port, printEngine, offset, pageNumber);
			this.PaintExpired(port, pageNumber);
			port.Transform = initialTransform;
		}

		protected void PaintExpired(Drawing.IPaintPort port, int pageNumber)
		{
			//	Dessine le warning d'installation.
			Size ds = this.document.GetPageSize(pageNumber);
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

		protected void InitSimplyPort(Printing.PrintPort port, PrintEngine printEngine, Point offset, int pageNumber)
		{
			//	Initialise le port pour une impression simplifiée.
			double zoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				double pw = printEngine.PaperSize.Width;
				double ph = printEngine.PaperSize.Height;
				if ( printEngine.Landscape )
				{
					Misc.Swap(ref pw, ref ph);
				}
				double zoomH = pw / this.document.GetPageSize(pageNumber).Width;
				double zoomV = ph / this.document.GetPageSize(pageNumber).Height;
				zoom = System.Math.Min(zoomH, zoomV);
			}
			else
			{
				zoom = 0.1*this.PrintInfo.Zoom;
			}
			port.ScaleTransform(zoom, zoom, 0, 0);
			port.TranslateTransform(offset.X, offset.Y);
		}


		protected string ExportGeometry(DrawingContext drawingContext, string filename, int pageNumber)
		{
			//	Exporte la géométrie complexe de tous les objets, en utilisant
			//	un bitmap intermédiaire.
			byte[] data;
			string err = this.ExportGeometry(drawingContext, pageNumber, this.imageFormat, this.imageDpi, this.imageCompression, this.imageDepth, this.imageQuality, this.imageAA, true, this.ImageOnlySelected, this.ImageCrop, out data);
			if (err != "")
			{
				return err;
			}

			try
			{
				System.IO.File.WriteAllBytes(filename, data);
			}
			catch (System.Exception e)
			{
				return e.Message;
			}

			return "";  // ok
		}

        protected string ExportGeometryICO(DrawingContext drawingContext, string filename, int pageNumber)
        {
            //	Exporte la géométrie complexe de tous les objets, en utilisant
            //	un bitmap intermédiaire.
            Settings.ExportICOInfo info = this.document.Settings.ExportICOInfo;
            Size pageSize = this.document.GetPageSize(pageNumber);
            ImageFormat format;
            double dpi;

            if (info.Format == Settings.ICOFormat.Vista)
            {
                format = ImageFormat.WindowsPngIcon;
                dpi = 256 * 2 * 254 / pageSize.Height;  // bitmap d'une hauteur de 256*2 pixels
            }
            else
            {
                format = ImageFormat.WindowsIcon;
				dpi = 48 *2 * 254 / pageSize.Height;  // bitmap d'une hauteur de 48*2 pixels
            }

            byte[] data;
            string err = this.ExportGeometry(drawingContext, pageNumber, format, dpi, ImageCompression.None, 32, 1.0, 1.0, true, this.ImageOnlySelected, this.ImageCrop, out data);
            if (err != "")
            {
                return err;
            }

            try
            {
                System.IO.File.WriteAllBytes(filename, data);
            }
            catch (System.Exception e)
            {
                return e.Message;
            }

            return "";  // ok
        }

        protected string ExportGeometry(DrawingContext drawingContext, int pageNumber, ImageFormat format, double dpi, ImageCompression compression, int depth, double quality, double AA, bool paintMark, bool onlySelected, ExportImageCrop crop, out byte[] data)
		{
			//	Exporte la géométrie complexe de tous les objets, en utilisant
			//	un bitmap intermédiaire. Retourne un éventuel message d'erreur ainsi
			//	que le tableau de bytes pour le fichier.
			data = null;

			if (format == ImageFormat.Unknown)
			{
				return Res.Strings.Error.BadImage;
			}

			Bitmap bitmap = this.ExportBitmap(drawingContext, pageNumber, -1, dpi, depth, AA, paintMark, onlySelected, crop);
			
			if (bitmap == null)
			{
				return Res.Strings.Error.NoBitmap;
			}

			try
			{
				int q = (int) System.Math.Round(quality*100.0);  // 0..100
				data = bitmap.Save(format, depth, q, compression, dpi);
			}
			catch (System.Exception e)
			{
				return e.Message;
			}

			return "";  // ok
		}

		protected Bitmap ExportBitmap(DrawingContext drawingContext, int pageNumber, int layerNumber, double dpi, int depth, double AA, bool paintMark, bool onlySelected, ExportImageCrop crop)
		{
			//	Retourne le bitmap contenant le dessin des objets à exporter.
			Rectangle pageBox;
			double zoom = dpi/(10.0*25.4);

			if (crop == ExportImageCrop.Page)
			{
				pageBox = new Rectangle(Point.Zero, this.document.GetPageSize(pageNumber));
			}
			else
			{
				pageBox = this.GetBoundingBox(pageNumber, crop == ExportImageCrop.Selection);
				if (pageBox.IsEmpty)  // aucun objet ?
				{
					return null;
				}
				pageBox.Inflate(zoom);
			}

			Rectangle pageScale = pageBox;
			pageScale.Scale(zoom);

			//	Il faut subtilement agrandir le rectangle, afin qu'un trait antialiasé soit contenu intégralement.
			int left   = (int) System.Math.Floor(pageScale.Left);
			int right  = (int) System.Math.Ceiling(pageScale.Right);
			int bottom = (int) System.Math.Floor(pageScale.Bottom);
			int top    = (int) System.Math.Ceiling(pageScale.Top);

			int dx = right-left;
			int dy = top-bottom;

			Graphics gfx = new Graphics();
			gfx.SetPixmapSize(dx, dy);
			gfx.SolidRenderer.ClearAlphaRgb((depth==32)?0:1, 1, 1, 1);
			gfx.Rasterizer.Gamma = AA;

			gfx.TranslateTransform(-left, bottom+dy);
			gfx.ScaleTransform(zoom, -zoom, 0, 0);

			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			for (int l=0; l<layers.Count; l++)
			{
				Objects.Layer layer = layers[l] as Objects.Layer;

				if (layerNumber != -1 && l != layerNumber)
				{
					continue;
				}

				Properties.ModColor modColor = layer.PropertyModColor;
				gfx.PushColorModifier (new ColorModifierCallback(modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				gfx.PushColorModifier (new ColorModifierCallback(drawingContext.DimmedColor));

				foreach (Objects.Abstract obj in this.document.Deep(layer, onlySelected))
				{
					if (obj.IsHide)
					{
						continue;  // objet caché ?
					}

					obj.DrawGeometry(gfx, drawingContext);
				}

				gfx.PopColorModifier();
				gfx.PopColorModifier();
			}

			if (drawingContext.GridShow)
			{
				gfx.LineWidth = 1/zoom;
				this.document.Modifier.ActiveViewer.DrawGuides(gfx);
				gfx.LineWidth = 1;
			}

			if (paintMark)
			{
				if (this.document.InstallType == InstallType.Demo)
				{
					this.PaintDemo(gfx, pageNumber);
				}
				if (this.document.InstallType == InstallType.Expired)
				{
					this.PaintExpired(gfx, pageNumber);
				}
			}

			Bitmap bitmap = Bitmap.FromPixmap(gfx.Pixmap) as Bitmap;
			return bitmap;
		}

		protected Rectangle GetBoundingBox(int pageNumber, bool onlySelected)
		{
			//	Retourne le rectangle englobant les objets à imprimer.
			Rectangle bbox = Rectangle.Empty;

			System.Collections.ArrayList layers = this.ComputeLayers(pageNumber);
			foreach (Objects.Layer layer in layers)
			{
				foreach (Objects.Abstract obj in this.document.Deep(layer, onlySelected))
				{
					if (obj.IsHide)
					{
						continue;  // objet caché ?
					}

					bbox = Rectangle.Union(bbox, obj.BoundingBoxGeom);
				}
			}

			return bbox;
		}


		protected Settings.PrintInfo PrintInfo
		{
			//	Donne les réglages de l'impression.
			get
			{
				return this.document.Settings.PrintInfo;
			}
		}


		protected Document					document;
		protected ExportImageCrop			imageCrop;
		protected bool						imageOnlySelected;
		protected ImageFormat				imageFormat;
		protected double					imageDpi;
		protected ImageCompression			imageCompression;
		protected int						imageDepth;
		protected double					imageQuality;
		protected double					imageAA;
		protected string[]					imageNameFilters;
	}
}
